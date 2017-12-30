#getting date and time
import datetime
#for sql database
import MySQLdb
#for parsing incoming data
from cgi import parse_qs, escape
#for parsing json strings
import json
#for email
from email.MIMEMultipart import MIMEMultipart
from email.MIMEText import MIMEText
import smtplib
#for random
import sys
import random

'''
STRUCTURE OF RESPONSE JSON
'{
	"Success" : <True | False>
	"Response" : "<response text, optional>"
	"Data" : <extra data, optional> #if nothing, then use "" or None, both will be treated as an empty string
}'
'''
def getResponseJSON(Success, Response, Data):
	if Data == None or Data == "":
		Data = '""'
	return '{"Success":' + str(Success).lower() + ', "Response":"' + Response.replace('"', '\\\"') + '", "Data":' + Data + '}'



#entry point
def application(environ, start_response):
	status = "200 OK"
	response = getResponseJSON(False, "Error: application: Unknown error, no start!", None)

	response_header = [('Content-type','text/plain')]
        start_response(status, response_header)
	
	# This will be the response if somebody tries to type the ip in a web browser
	if environ["REQUEST_METHOD"] == "GET":
		return "This is a backend server, not a website you goof!"
	
	# the environment variable CONTENT_LENGTH may be empty or missing
	try:
		request_body_size = int(environ.get('CONTENT_LENGTH', 0))
	except ValueError as e:
		request_body_size = 0
		error = "Error:application: Empty request body:" + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	
	#just in case drop_user had problems and didn't set it back to 1
	db = MySQLdb.connect("localhost","testuser","cs441","mysql")
	cursor = db.cursor()
	cursor.execute("SET foreign_key_checks = 1")
	
	# When the method is POST the variable will be sent
	# in the HTTP request body which is passed by the WSGI server
	# in the file like wsgi.input environment variable.
	request_body = environ['wsgi.input'].read(request_body_size)
	d = json.loads(request_body)
	print(d)
	
	if d["Command"] == "ADD_USER": #done
		response = add_new_user(d)
	elif d["Command"] == "VALIDATE_USER": #done
		response = validate_user(d)
	elif d["Command"] == "GET_USERS": #done
		response = get_users(d)
	#edit user?
	
	elif d["Command"] == "FORGOT_PASSWORD": #done
		response = forgot_password(d)
	elif d["Command"] == "INVITE_USER": #done
		response = invite_user(d)
	elif d["Command"] == "DROP_USER": #done
		response = drop_user(d)
	
	elif d["Command"] == "GET_USER_CLASSROOMS": #done
		response = get_user_classrooms(d)
	elif d["Command"] == "CREATE_CLASSROOM": #done
		response = create_classroom(d)
	elif d["Command"] == "JOIN_CLASSROOM": #done
		response = join_classroom(d)
	elif d["Command"] == "LEAVE_CLASSROOM": #done
		response = leave_classroom(d)
	elif d["Command"] == "DELETE_CLASSROOM": #done
		response = delete_classroom(d)
	elif d["Command"] == "EDIT_CLASSROOM": #done
		response = edit_classroom(d)
	
	elif d["Command"] == "GET_TODO_ITEMS": #done
		response = get_todo_items(d)
	elif d["Command"] == "CREATE_TODO_ITEM": #done
		response = create_todo_item(d)
	elif d["Command"] == "DELETE_TODO_ITEM": #done
		response = delete_todo_item(d)
	elif d["Command"] == "EDIT_TODO_ITEM": #done
		response = edit_todo_item(d)
	
	elif d["Command"] == "GET_FORUM_THREADS": #done
		response = get_forum_threads(d)
	elif d["Command"] == "CREATE_FORUM_THREAD": #done
		response = create_forum_thread(d)
	elif d["Command"] == "DELETE_FORUM_THREAD": #done
		response = delete_forum_thread(d)
	elif d["Command"] == "EDIT_FORUM_THREAD": #done, removed
		response = edit_forum_thread(d)
	
	elif d["Command"] == "GET_THREAD_POSTS": #done
		response = get_thread_posts(d)
	elif d["Command"] == "CREATE_THREAD_POST": #done
		response = create_thread_post(d)
	elif d["Command"] == "DELETE_THREAD_POST": #done
		response = delete_thread_post(d)
	elif d["Command"] == "EDIT_THREAD_POST": #done
		response = edit_thread_post(d)
	
	elif d["Command"] == "CREATE_CHATROOM": #done
		response = create_chatroom(d)
	elif d["Command"] == "LEAVE_CHATROOM": #done
		response = leave_chatroom(d)
	elif d["Command"] == "POST_CHATROOM_MESSAGE": #done
		response = post_chatroom_message(d)
	elif d["Command"] == "GET_USER_CHATROOMS": #done
		response = get_user_chatrooms(d)
	elif d["Command"] == "GET_CHATROOM_MESSAGES": #done
		response = get_chatroom_messages(d)
	
	return response



def get_forum_threads(data):
	responseData = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if part of classroom and check if correct login info
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Users.Id "
		"FROM Users, R_UserClassrooms "
		"WHERE Users.Id=UserId "
		"AND Email='"      + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Could not find a user with the given login info that is a member of the given classroom")
		# get threads
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT ForumThreads.Id, Title, ForumThreads.CreatorId, Name, Email, ForumThreads.CreatedDateTime, COUNT(ThreadId), MAX(ForumPosts.CreatedDateTime) "
		"FROM ForumThreads, main_db.Users, ForumPosts "
		"WHERE Users.Id=ForumThreads.CreatorId "
		"AND ThreadId=ForumThreads.Id "
		"GROUP BY ForumThreads.Id")
		result = cursor.fetchall()
		# make the data object to be returned
		if len(result) > 0:
			responseData = "'["
			for row in result:
				responseData += (
				"{\\\"Id\\\":\\\""              + str(row[0]) + "\\\","
				"\\\"Title\\\":\\\""            + str(row[1]) + "\\\","
				"\\\"CreatorId\\\":\\\""        + str(row[2]) + "\\\","
				"\\\"CreatorName\\\":\\\""      + str(row[3]) + "\\\","
				"\\\"CreatorEmail\\\":\\\""     + str(row[4]) + "\\\","
				"\\\"CreatedDateTime\\\":\\\""  + str(row[5]) + "\\\","
				"\\\"NumberOfPosts\\\":\\\""    + str(row[6]) + "\\\","
				"\\\"LastPostDateTime\\\":\\\"" + str(row[7]) + "\\\"},")
			responseData = responseData[0:-1] #removes last unnecessary comma
			responseData += "]'"
	except Exception as e:
		db.close()
		error = "Error retrieving forum threads: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Successfully retrieved forum threads", responseData)



def create_forum_thread(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if logged in and part of the classroom
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to create a forum thread")
		userId = result[0];
		# add forum thread and post to tables
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute("INSERT INTO ForumThreads(Title,CreatorId,CreatedDateTime,LastEditDateTime) VALUES ('" + str(data["Title"]) + "', '" + str(userId) + "', '" + str(datetime.datetime.now()) + "', '" + str(datetime.datetime.now()) + "')")
		cursor.execute("INSERT INTO ForumPosts(ThreadId,CreatorId,Content,CreatedDateTime,LastEditDateTime) VALUES ('" + str(cursor.lastrowid) + "', '" + str(userId) + "', '" + str(data["Content"]) + "', '" + str(datetime.datetime.now()) + "', '" + str(datetime.datetime.now()) + "')")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error creating forum thread: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Forum thread successfully created", None)



def delete_forum_thread(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if logged in and part of the classroom
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to delete a forum thread")
		userId = result[0];
		# check if allowed to delete thread
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT CreatorId "
		"FROM ForumThreads "
		"WHERE Id=" + str(data["ThreadId"]) + "")
		result = cursor.fetchone()
		creatorId = result[0]
		if creatorId != userId: # not creator, then must be owner
			cursor.execute(
			"SELECT OwnerId "
			"FROM main_db.Classrooms "
			"WHERE Id=" + str(data["DatabaseId"]) + "")
			cursor.fetchone()
			if cursor.rowcount == 0:
				raise Exception("You cannot delete a thread that you did not create if you are not the instructor")
		# delete forum thread from table
		cursor.execute(
		"DELETE FROM ForumThreads "
		"WHERE Id=" + str(data["ThreadId"]) + "")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error deleting forum thread: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Forum thread successfully deleted", None)



# This isn't necessary. When you edit a post and it is the first post, then edit a thread's title
def edit_forum_thread(data):
	return getResponseJSON(False, "EDIT_FORUM_THREAD not implemented", None)
'''	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if logged in and part of the classroom
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to edit a forum thread")
		userId = result[0];
		# check if allowed to edit thread
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT CreatorId "
		"FROM ForumThreads "
		"WHERE Id=" + str(data["ThreadId"]) + "")
		result = cursor.fetchone()
		creatorId = result[0]
		if creatorId != userId: # not creator, then must be owner
			cursor.execute(
			"SELECT OwnerId "
			"FROM main_db.Classrooms "
			"WHERE Id=" + str(data["DatabaseId"]) + "")
			cursor.fetchone()
			if cursor.rowcount == 0:
				raise Exception("You cannot edit a thread that you did not create if you are not the instructor")
		# add item to table
		cursor.execute(
		"UPDATE ForumThreads "
		"SET Title='"        + str(data["Title"]) + "', "
		"LastEditDateTime='" + str(datetime.datetime.now()) + "' "
		"WHERE Id="          + str(data["ThreadId"]) + "")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error editing forum thread: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Forum thread successfully edited", None)'''



def get_thread_posts(data):
	responseData = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if logged in and part of the classroom
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to get forum thread posts")
		userId = result[0]
		# get posts
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT ForumPosts.Id,Name,Email,Content,CreatedDateTime,LastEditDateTime "
		"FROM main_db.Users,ForumPosts "
		"WHERE Users.Id=CreatorId "
		"AND ThreadId=" + str(data["ThreadId"]) + " "
		"ORDER BY CreatedDateTime ASC")
		result = cursor.fetchall()
		# make the data object to be returned
		if len(result) > 0:
			responseData = "'["
			for row in result:
				responseData += (
				"{\\\"Id\\\":\\\""              + str(row[0]) + "\\\","
				"\\\"CreatorName\\\":\\\""      + str(row[1]) + "\\\","
				"\\\"CreatorEmail\\\":\\\""     + str(row[2]) + "\\\","
				"\\\"Content\\\":\\\""          + str(row[3]) + "\\\","
				"\\\"CreatedDateTime\\\":\\\""  + str(row[4]) + "\\\","
				"\\\"LastEditDateTime\\\":\\\"" + str(row[5]) + "\\\"},")
			responseData = responseData[0:-1] #removes last unnecessary comma
			responseData += "]'"
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error retrieving forum thread posts: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
    	
	return getResponseJSON(True, "Successfully retrieved forum thread posts", responseData)



def create_thread_post(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if logged in and part of the classroom
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to create a forum thread post")
		userId = result[0];
		# add forum thread to table
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute("INSERT INTO ForumPosts(ThreadId,CreatorId,Content,CreatedDateTime,LastEditDateTime) VALUES ('" + str(data["ThreadId"]) + "', '" + str(userId) + "', '" + str(data["Content"]) + "', '" + str(datetime.datetime.now()) + "', '" + str(datetime.datetime.now()) + "')")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error creating forum thread post: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Forum thread post successfully created", None)



def delete_thread_post(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if logged in and part of the classroom
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to delete a forum thread post")
		userId = result[0];
		# check if allowed to delete thread
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT CreatorId "
		"FROM ForumPosts "
		"WHERE Id=" + str(data["PostId"]) + "")
		result = cursor.fetchone()
		creatorId = result[0]
		if creatorId != userId: # not creator, then must be owner
			cursor.execute(
			"SELECT OwnerId "
			"FROM main_db.Classrooms "
			"WHERE Id=" + str(data["DatabaseId"]) + "")
			cursor.fetchone()
			if cursor.rowcount == 0:
				raise Exception("You cannot delete a forum thread post that you did not create if you are not the instructor")
		# delete forum thread from table
		cursor.execute(
		"DELETE FROM ForumPosts "
		"WHERE Id=" + str(data["PostId"]) + "")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error deleting forum thread post: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Forum thread post successfully deleted", None)



def edit_thread_post(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("Use main_db")
		# check if logged in and part of the classroom
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to edit a forum thread post")
		userId = result[0];
		# check if allowed to edit thread
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT CreatorId "
		"FROM ForumPosts "
		"WHERE Id=" + str(data["PostId"]) + "")
		result = cursor.fetchone()
		creatorId = result[0]
		if creatorId != userId: # not creator, then must be owner
			cursor.execute(
			"SELECT OwnerId "
			"FROM main_db.Classrooms "
			"WHERE Id=" + str(data["DatabaseId"]) + "")
			cursor.fetchone()
			if cursor.rowcount == 0:
				raise Exception("You cannot edit a thread post that you did not create if you are not the instructor")
		# edit item in table
		cursor.execute(
		"SELECT ForumThreads.Id "
		"FROM ForumPosts,ForumThreads "
		"WHERE ForumPosts.CreatedDateTime=("
		"  SELECT MIN(CreatedDateTime) "
		"  FROM ForumPosts "
		"  WHERE ThreadId=("
		"    SELECT ThreadId "
		"    FROM ForumPosts "
		"    WHERE Id=" + str(data["PostId"]) + ""
		"  )"
		") "
		"AND ForumPosts.Id=" + str(data["PostId"]) + " "
		"AND ForumThreads.Id=ThreadId")
		result = cursor.fetchone()
		if cursor.rowcount == 1:
			threadId = result[0]
			cursor.execute(
			"UPDATE ForumThreads "
			"SET Title='"        + str(data["Title"]) + "', "
			"LastEditDateTime='" + str(datetime.datetime.now()) + "' "
			"WHERE Id="          + str(threadId) + "")
		cursor.execute(
		"UPDATE ForumPosts "
		"SET Content='"      + str(data["Content"]) + "', "
		"LastEditDateTime='" + str(datetime.datetime.now()) + "' "
		"WHERE Id="          + str(data["PostId"]) + "")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error editing forum thread: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Forum thread post successfully edited", None)



def create_chatroom(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# check there are enough members to create a chatroom
		if len(data["MemberIds"]) < 2:
			raise Exception("A chatroom cannot contains less than 2 members")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if logged in and part of the classroom
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to create a chatroom")
		userId = result[0];
		# add chatroom to tables
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute("INSERT INTO Chatrooms() VALUES()")
		threadId = cursor.lastrowid;
		for row in data["MemberIds"]:
			cursor.execute("INSERT INTO R_ChatroomMembers(ChatroomId, MemberId) VALUES(" + str(threadId) + "," + str(row) + ")")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error creating chatroom: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Chatroom successfully created", None)



def leave_chatroom(data):
	deleteResponse = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if logged in and part of the classroom
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to leave a chatroom")
		userId = result[0]
		# check if part of chatroom
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT MemberId "
		"FROM R_ChatroomMembers "
		"WHERE MemberId=" + str(userId) + " "
		"AND ChatroomId=" + str(data["ChatroomId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("User is not in chatroom. You must be a part of a chatroom to leave it")
		# delete user from the relationship table
		cursor.execute(
		"DELETE "
		"FROM R_ChatroomMembers "
		"WHERE MemberId=" + str(userId) + " "
		"AND ChatroomId=" + str(data["ChatroomId"]) + "")
		# if no more members of chatroom, delete id in chatrooms table
		cursor.execute(
		"SELECT ChatroomId "
		"FROM R_ChatroomMembers "
		"WHERE ChatroomId=" + str(data["ChatroomId"]) + "")
		cursor.fetchall()
		if cursor.rowcount == 0:
			cursor.execute(
			"DELETE "
			"FROM Chatrooms "
			"WHERE Id=" + str(data["ChatroomId"]) + "")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error leaving chatroom: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	
	db.close()
	return getResponseJSON(True, "User successfully left chatroom", None)



def post_chatroom_message(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if logged in and part of the classroom
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to post a chatroom message")
		userId = result[0]
		# check if part of chatroom
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT MemberId "
		"FROM R_ChatroomMembers "
		"WHERE MemberId=" + str(userId) + " "
		"AND ChatroomId=" + str(data["ChatroomId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("User is not in chatroom. You must be a part of a chatroom to post a message in it")
		# add message to table
		cursor.execute("INSERT INTO ChatroomMessages(ChatroomId, Message, CreatedDateTime, CreatorId) VALUES ('" + str(data["ChatroomId"]) + "', '" + str(data["Message"]) + "', '" + str(datetime.datetime.now()) + "', '" + str(userId) + "')")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error posting chatroom message: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
    	
	return getResponseJSON(True, "Successfully posted chatroom message", None)



def get_chatroom_messages(data):
	responseData = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if logged in and part of the classroom
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Id "
		"FROM Users,R_UserClassrooms "
		"WHERE Email='"    + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND UserId=Id "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email, password, and/or classroom are incorrect. You must be logged in and part of the classroom to get chatroom messages")
		userId = result[0]
		# check if part of chatroom
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT MemberId "
		"FROM R_ChatroomMembers "
		"WHERE MemberId=" + str(userId) + " "
		"AND ChatroomId=" + str(data["ChatroomId"]) + "")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("User is not in chatroom. You must be a part of a chatroom to get messages in it")
		# get messages
		cursor.execute(
		"SELECT Message,CreatorId,Name,Email,CreatedDateTime "
		"FROM main_db.Users,ChatroomMessages "
		"WHERE Users.Id=CreatorId "
		"AND ChatroomId=" + str(data["ChatroomId"]) + " "
		"ORDER BY CreatedDateTime ASC")
		result = cursor.fetchall()
		# make the data object to be returned
		if len(result) > 0:
			responseData = "'["
			for row in result:
				responseData += (
				"{\\\"Message\\\":\\\""        + str(row[0]) + "\\\","
				"\\\"CreatorId\\\":\\\""       + str(row[1]) + "\\\","
				"\\\"CreatorName\\\":\\\""     + str(row[2]) + "\\\","
				"\\\"CreatorEmail\\\":\\\""    + str(row[3]) + "\\\","
				"\\\"CreatedDateTime\\\":\\\"" + str(row[4]) + "\\\"},")
			responseData = responseData[0:-1] #removes last unnecessary comma
			responseData += "]'"
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error retrieving chatroom messages: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
    	
	return getResponseJSON(True, "Successfully retrieved chatroom messages", responseData)



def get_user_chatrooms(data):
	responseData = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check user is in classroom
		cursor.execute("USE main_db")
		cursor.execute("SELECT Id "
		"FROM Users, R_UserClassrooms "
		"WHERE Id=UserId "
		"AND ClassroomId=" + str(data["DatabaseId"]) + " "
		"AND Email='"      + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "'")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("The user does not exist or the user is not a member of the classroom")
		userId = result[0]
		# get chatrooms that user is a part of
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT ChatroomId "
		"FROM R_ChatroomMembers "
		"WHERE MemberId=" + str(userId) + "")
		result = cursor.fetchall()
		# use the chatroom ids to get all chatrooms and members involved
		if cursor.rowcount > 0:
			chatroomIds = ""
			for row in result:
				chatroomIds += str(row[0]) + ","
			chatroomIds = chatroomIds[0:-1] #removes last unnecessary comma
			cursor.execute(
			"SELECT ChatroomId,MemberId,Name,Email "
			"FROM R_ChatroomMembers,main_db.Users "
			"WHERE ChatroomId IN (" + chatroomIds + ") "
			"AND Id=MemberId "
			"ORDER BY ChatroomId ASC")
			result = cursor.fetchall()
			
			# make the data object to be returned
			if cursor.rowcount > 0:
				curChatroomId = -1
				responseData = "'["
				for row in result:
					if row[0] != curChatroomId:
						if curChatroomId != -1:
							responseData = responseData[0:-1] #removes last unnecessary comma
							responseData += "]},"
						curChatroomId = row[0]
						responseData += (
						"{\\\"Id\\\":\\\"" + str(row[0]) + "\\\","
						"\\\"Members\\\":[")
					responseData += (
					"{\\\"Id\\\":\\\""     + str(row[1]) + "\\\","
					"\\\"Name\\\":\\\""    + str(row[2]) + "\\\","
					"\\\"Email\\\":\\\""   + str(row[3]) + "\\\"},")
				responseData = responseData[0:-1] #removes last unnecessary comma
				responseData += "]}]'"
				
				#works #responseData = "'[{\\\"Id\\\":\\\"0\\\",\\\"Members\\\":[{\\\"Id\\\":\\\"0\\\",\\\"Name\\\":\\\"James\\\",\\\"Email\\\":\\\"JamesEmail\\\"}]}]'"
				#print(responseData)
				#print(responseData.replace("\\",""))
	except Exception as e:
		db.close()
		error = "Error getting user chatrooms: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "User chatrooms successfully retrieved", responseData)



def add_new_user(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		cursor.execute("INSERT INTO Users(Name, Email, Password) VALUES('" + str(data["Name"]) + "','" + str(data["Email"]) + "','" + str(data["Password"])  + "')")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error adding new user to database: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
    # disconnect from server
	db.close()
	
	return getResponseJSON(True, "User successfully added to database", None)



def validate_user(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		cursor.execute("SELECT Id FROM Users WHERE Email='" + str(data["Email"]) + "' AND Password='" + str(data["Password"]) + "'")
		# fetch a single row
		result = cursor.fetchone()
		# if there is an entry in the database, then the email and password match, else it is incorrect or non-existant
		if cursor.rowcount == 0:
			raise Exception("Email and/or password are incorrect")
		# commit changes
		#db.commit()
	except Exception as e:
		#db.rollback()
		db.close()
		error = "Error validating user: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "User successfully validated", '"[' + str(result[0]) + ']"') #result[0] is Id



def get_users(data):
	responseData = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if part of classroom and check if correct login info
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Users.Id "
		"FROM Users, R_UserClassrooms "
		"WHERE Users.Id=UserId "
		"AND Email='"      + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "' "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Could not find a user with the given login info that is a member of the given classroom")
		# get users
		cursor.execute(
		"SELECT Id, Name, Email "
		"FROM Users, R_UserClassrooms "
		"WHERE Id=UserId "
		"AND ClassroomId=" + str(data["DatabaseId"]) + "")
		result = cursor.fetchall()
		# make the data object to be returned
		if len(result) > 0:
			responseData = "'["
			for row in result:
				responseData += (
				"{\\\"Id\\\":\\\""   + str(row[0]) + "\\\","
				"\\\"Name\\\":\\\""  + str(row[1]) + "\\\","
				"\\\"Email\\\":\\\"" + str(row[2]) + "\\\"},")
			responseData = responseData[0:-1] #removes last unnecessary comma
			responseData += "]'"
	except Exception as e:
		db.close()
		error = "Error retrieving users: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Successfully retrieved users", responseData)



def create_classroom(data):
	db_header = "Classroom_db_"
	
	rng = random.SystemRandom()
	rng.random()
	
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# verify database name isn't taken
		attempts = 0
		maxAttempts = 10
		while attempts < maxAttempts:
			attempts += 1
			classroomId = str(rng.randint(0, 2**32-1)).rjust(10,'0') #right pad with 0's to look nice
			cursor.execute("SHOW DATABASES LIKE '" + db_header + classroomId + "'")
			cursor.fetchone()
			if cursor.rowcount == 0:
				break
		if attempts == maxAttempts:
			raise Exception("Server could not create a database with unique id!")
		# get user id
		cursor.execute("USE main_db")
		cursor.execute("SELECT Id FROM Users WHERE Email='" + str(data["Email"]) + "' AND Password='" + str(data["Password"]) + "'")
		userId = cursor.fetchone()[0]
		# add classroom to Classrooms table
		cursor.execute("INSERT INTO Classrooms(Id, Title, Description, OwnerId) VALUES(" + classroomId + ", '" + str(data["Title"]) + "', '" + str(data["Description"]) + "', " + str(userId) + ")")
		# add classroom, user combination to R_UserClassrooms table
		cursor.execute("INSERT INTO R_UserClassrooms(UserId, ClassroomId) VALUES(" + str(userId) + ", " + classroomId + ")")
		# add tables
		cursor.execute("CREATE DATABASE " + db_header + classroomId)
		cursor.execute("USE Classroom_db_" + classroomId)
		cursor.execute(tableMySQL_Members)
		cursor.execute(tableMySQL_Chatrooms)
		cursor.execute(tableMySQL_ChatroomMessages)
		cursor.execute(tableMySQL_R_ChatroomMembers)
		cursor.execute(tableMySQL_TodoItems)
		cursor.execute(tableMySQL_ForumThreads)
		cursor.execute(tableMySQL_ForumPosts)
		# add owner to Members table
		cursor.execute("INSERT INTO Members(UserId) VALUES(" + str(userId) + ")")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		# delete database if it was created, rollback may not work in this situation
		db.close()
		error = "Error creating classroom: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Classroom successfully created", None)



def edit_classroom(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if owner of classroom and check if correct login info
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Users.Id "
		"FROM Users, Classrooms "
		"WHERE Users.Id=OwnerId "
		"AND Email='"        + str(data["Email"]) + "' "
		"AND Password='"     + str(data["Password"]) + "' "
		"AND Classrooms.Id=" + str(data["DatabaseId"]) + "")
		cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Could not find a user with the given login info that is the owner of the given classroom")
		# add todo item to table
		cursor.execute(
		"UPDATE Classrooms "
		"SET Title='"   + str(data["Title"]) + "',"
		"Description='" + str(data["Description"]) + "' "
		"WHERE Id="     + str(data["DatabaseId"]) + "")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error editing classroom: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Classroom successfully edited", None)



def create_todo_item(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if owner of classroom and check if correct login info
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Users.Id "
		"FROM Users, Classrooms "
		"WHERE Users.Id=OwnerId "
		"AND Email='"        + str(data["Email"]) + "' "
		"AND Password='"     + str(data["Password"]) + "' "
		"AND Classrooms.Id=" + str(data["DatabaseId"]))
		cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Could not find a user with the given login info that is the owner of the given classroom")
		# add todo item to table
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute("INSERT INTO TodoItems(Title, Description, DueDateTime, CreatedDateTime, LastEditDateTime) VALUES ('" + str(data["Title"]) + "', '" + str(data["Description"]) + "', '" + str(data["DueDateTime"]) + "', '" + str(datetime.datetime.now()) + "', '" + str(datetime.datetime.now()) + "')")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error creating todo item: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
    	
	return getResponseJSON(True, "Todo item successfully created", None)



def delete_todo_item(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if owner of classroom and check if correct login info
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Users.Id "
		"FROM Users, Classrooms "
		"WHERE Users.Id=OwnerId "
		"AND Email='"        + str(data["Email"]) + "' "
		"AND Password='"     + str(data["Password"]) + "' "
		"AND Classrooms.Id=" + str(data["DatabaseId"]) + "")
		cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("The user does not exist or is not the owner of the classroom")
		# add todo item to table
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute("DELETE FROM TodoItems WHERE Id=" + str(data["TodoItemId"]))
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error deleting todo item: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Todo item successfully deleted", None)



def get_todo_items(data):
	responseData = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check user is in classroom
		cursor.execute("USE main_db")
		cursor.execute("SELECT Id "
		"FROM Users, R_UserClassrooms "
		"WHERE Id=UserId "
		"AND ClassroomId=" + str(data["DatabaseId"]) + " "
		"AND Email='"      + str(data["Email"]) + "' "
		"AND Password='"   + str(data["Password"]) + "'")
		cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("The user does not exist or the user is not a member of the classroom")
		# get todo items
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
		cursor.execute(
		"SELECT Id,Title,Description,DueDateTime,CreatedDateTime,LastEditDateTime "
		"FROM TodoItems "
		"ORDER BY DueDateTime ASC")
		result = cursor.fetchall()
		
		# make the data object to be returned
		if len(result) > 0:
			responseData = "'["
			for row in result:
				responseData += (
				"{\\\"Id\\\":\\\""              + str(row[0]) + "\\\","
				"\\\"Title\\\":\\\""            + str(row[1]) + "\\\","
				"\\\"Description\\\":\\\""      + str(row[2]) + "\\\","
				"\\\"DueDateTime\\\":\\\""      + str(row[3]) + "\\\","
				"\\\"CreatedDateTime\\\":\\\""  + str(row[4]) + "\\\","
				"\\\"LastEditDateTime\\\":\\\"" + str(row[5]) + "\\\"},")
			responseData = responseData[0:-1] #removes last unnecessary comma
			responseData += "]'"
	except Exception as e:
		db.close()
		error = "Error getting todo items: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Todo items successfully retrieved", responseData)



def edit_todo_item(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		# check if owner of classroom and check if correct login info
		cursor.execute("USE main_db")
		cursor.execute(
		"SELECT Users.Id "
		"FROM Users, Classrooms "
		"WHERE Users.Id=OwnerId "
		"AND Email='"        + str(data["Email"]) + "' "
		"AND Password='"     + str(data["Password"]) + "' "
		"AND Classrooms.Id=" + str(data["DatabaseId"]) + "")
		cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Could not find a user with the given login info that is the owner of the given classroom")
		# add todo item to table
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10, '0'))
		cursor.execute(
		"UPDATE TodoItems "
		"SET Title='"        + str(data["Title"]) + "',"
		"Description='"      + str(data["Description"]) + "',"
		"DueDateTime='"      + str(data["DueDateTime"]) + "', "
		"LastEditDateTime='" + str(datetime.datetime.now()) + "' "
		"WHERE Id="          + str(data["TodoItemId"]) + "")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error editing todo item: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Todo item successfully edited", None)



def get_user_classrooms(data):
	responseData = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# get classrooms
		cursor.execute("SELECT Id FROM Users WHERE Email='" + str(data["Email"]) + "' AND Password='" + str(data["Password"]) + "'")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email and/or password are incorrect")
		cursor.execute(
		"SELECT Name,Email,Title,Description,Classrooms.Id "
		"FROM Classrooms,Users,R_UserClassrooms "
		"WHERE R_UserClassrooms.ClassroomId=Classrooms.Id "
		"AND R_UserClassrooms.UserId='" + str(result[0]) + "' "
		"AND Users.Id=Classrooms.OwnerId")
		result = cursor.fetchall()
		
		# make the data object to be returned
		if len(result) > 0:
			responseData = "'["
			for row in result:
				responseData += (
				"{\\\"OwnerName\\\":\\\""  + str(row[0]) + "\\\","
				"\\\"OwnerEmail\\\":\\\""  + str(row[1]) + "\\\","
				"\\\"Title\\\":\\\""       + str(row[2]) + "\\\","
				"\\\"Description\\\":\\\"" + str(row[3]) + "\\\","
				"\\\"Id\\\":\\\""          + str(row[4]).rjust(10, '0') + "\\\"},")
			responseData = responseData[0:-1] #removes last unnecessary comma
			responseData += "]'"
	except Exception as e:
		#db.rollback()
		db.close()
		error = "Error getting classrooms: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "Classroom successfully created", responseData)



def join_classroom(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		cursor.execute("SELECT Id FROM Users WHERE Email='" + str(data["Email"]) + "' AND Password='" + str(data["Password"]) + "'")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email and/or password are incorrect")
		cursor.execute(		
		"INSERT INTO R_UserClassrooms(UserId,ClassroomId) "
		"VALUES(" + str(result[0]) + "," + str(data["DatabaseId"]) + ")")
		cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10, '0'))
		cursor.execute(
		"INSERT INTO Members(UserId) "
		"VALUES(" + str(result[0]) + ")")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error joining classroom: "
		if e.args[0] == 1062:
			error += "You already belong to this classroom"
		elif e.args[0] == 1452:
			error += "Cannot find a classroom with that ID"
		else:
			error += str(e)
		print(error)
		return getResponseJSON(False, error, None)
	
	db.close()
	return getResponseJSON(True, "User successfully joined classroom", None)



def leave_classroom(data):
	deleteResponse = ""
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		cursor.execute("SELECT Id FROM Users WHERE Email='" + str(data["Email"]) + "' AND Password='" + str(data["Password"]) + "'")
		result = cursor.fetchone()
		if cursor.rowcount == 0:
			raise Exception("Email and/or password are incorrect. You must be logged in to leave a classroom")
		userId = result[0]
		# check if owner, if owner then delete classroom
		cursor.execute(
		"SELECT Users.Id "
		"FROM Users, Classrooms "
		"WHERE Users.Id=OwnerId "
		"AND Email='"        + str(data["Email"]) + "' "
		"AND Password='"     + str(data["Password"]) + "' "
		"AND Classrooms.Id=" + str(data["DatabaseId"]))
		result = cursor.fetchone()
		if cursor.rowcount == 1:
			db.close()
			return delete_classroom(data)
		else:
			# delete user from the relationship table, if the user is in it
			cursor.execute(
			"DELETE "
			"FROM R_UserClassrooms "
			"WHERE UserId="    + str(userId) + " "
			"AND ClassroomId=" + str(data["DatabaseId"]) + "")
			cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
			cursor.execute("SET foreign_key_checks = 0")
			cursor.execute(
			"DELETE "
			"FROM Members "
			"WHERE UserId=" + str(userId) + "")
			cursor.execute("SET foreign_key_checks = 1")
			# commit changes
			db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error leaving classroom: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	
	db.close()
	return getResponseJSON(True, "User successfully left classroom", None)



def delete_classroom(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if owner and valid login info
		cursor.execute(
		"SELECT Classrooms.Id "
		"FROM Classrooms,Users "
		"WHERE Classrooms.Id=" + str(data["DatabaseId"]) + " "
		"AND Email='"          + str(data["Email"]) + "' "
		"AND Password='"       + str(data["Password"]) + "' "
		"AND Users.Id=Classrooms.OwnerId")
		# fetch a single row using
		result = cursor.fetchone()
		if len(result) == 1:
			cursor.execute("DROP DATABASE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
			cursor.execute("DELETE FROM Classrooms WHERE Id=" + str(data["DatabaseId"]))
		else:
			raise Exception("Classroom not found")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error deleting classroom: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	
	db.close()
	return getResponseJSON(True, "Classroom successfully deleted", None)



def invite_user(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if owner and valid login info
		cursor.execute(
		"SELECT Users.Id, Name, Classrooms.Title "
		"FROM Users, Classrooms "
		"WHERE Users.Id=OwnerId "
		"AND Email='"        + str(data["Email"]) + "' "
		"AND Password='"     + str(data["Password"]) + "' "
		"AND Classrooms.Id=" + str(data["DatabaseId"]) + "")
		# fetch a single row using
		result = cursor.fetchone()
		if len(result) > 0:
			# check if invite user is not already in the classroom
			cursor.execute(
			"SELECT Id "
			"FROM Users, R_UserClassrooms "
			"WHERE Id=UserId "
			"AND Email='"      + str(data["InviteEmail"]) + "' "
			"AND ClassroomId=" + str(data["DatabaseId"]) + "")
			cursor.fetchone()
			if cursor.rowcount == 1:
				raise Exception("The user with the given email is already part of this classroom")
			# check if user exists
			cursor.execute(
			"SELECT Id "
			"FROM Users "
			"WHERE Email='" + str(data["InviteEmail"]) + "'")
			cursor.fetchone()
			if cursor.rowcount == 0:
				raise Exception("No user with the given email exists")
			send_email(str(data["InviteEmail"]), '<html><body><h3>You have been invited to the classroom "' + result[2] + '" by "' + result[1] + '".<br><br>You can join the classroom by logging in to the "My Classrooms" page, then clicking the "Join Classroom" icon. Then you enter this classroom ID: ' + str(data["DatabaseId"]) + '</h3></body></html>')
		else:
			raise Exception("Could not find a user with the given login info that is the owner of the given classroom")
	except Exception as e:
		db.close()
		error = "Error sending invite email: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()

	return getResponseJSON(True, "Invite email successfully sent", None)



def drop_user(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		# check if owner and valid login info
		cursor.execute(
		"SELECT Users.Id "
		"FROM Users, Classrooms "
		"WHERE Users.Id=OwnerId "
		"AND Email='"        + str(data["Email"]) + "' "
		"AND Password='"     + str(data["Password"]) + "' "
		"AND Classrooms.Id=" + str(data["DatabaseId"]) + "")
		# fetch a single row using
		result = cursor.fetchone()
		userId = result[0]
		if cursor.rowcount == 1:
			if str(userId) == data["DropId"]:
				raise Exception("Cannot drop yourself from your classroom");
			cursor.execute(
			"DELETE "
			"FROM R_UserClassrooms "
			"WHERE UserId=" + str(data["DropId"]) + "")
			cursor.execute("USE Classroom_db_" + str(data["DatabaseId"]).rjust(10,'0'))
			cursor.execute("SET foreign_key_checks = 0")
			cursor.execute(
			"DELETE "
			"FROM Members "
			"WHERE UserId=" + str(data["DropId"]) + "")
			cursor.execute("SET foreign_key_checks = 1")
		else:
			raise Exception("Could not find a user with the given login info that is the owner of the given classroom")
		# commit changes
		db.commit()
	except Exception as e:
		db.rollback()
		db.close()
		error = "Error dropping user: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	return getResponseJSON(True, "User successfully dropped", None)



def forgot_password(data):
	try:
		# open database connection
		db = MySQLdb.connect("localhost","testuser","cs441","mysql")
		# prepare a cursor object
		cursor = db.cursor()
		# execute SQL query
		cursor.execute("USE main_db")
		cursor.execute("SELECT Password FROM Users WHERE Email='" + str(data["Email"]) + "'")
		# fetch a single row using
		result = cursor.fetchone()
	except Exception as e:
		db.close()
		error = "Error sending recovery email: " + str(e)
		print(error)
		return getResponseJSON(False, error, None)
	# disconnect from server
	db.close()
	
	if cursor.rowcount == 1:
		try:
			send_email(str(data["Email"]), '<html><body><h1>Your password is: ' + result[0]  + '</h1></body></html>')
		except Exception as e:
			error2 = "Error sending recovery email: " + str(e)
			print(error2)
			return getResponseJSON(False, error2, None)
		return getResponseJSON(True, "Recovery email successfully sent", None)
	else:
		return getResponseJSON(False, "Error sending recovery email: Unknown email", None)



def send_email(recipient, message):
    gmailUser = 'testemail991991@gmail.com'
    gmailPassword = 'testemail991991password'

    msg = MIMEMultipart('alternative')
    msg['From'] = gmailUser
    msg['To'] = recipient
    msg['Subject'] = "Subject of the email"
    msg.attach(MIMEText(message,'html'))

    mailServer = smtplib.SMTP('smtp.gmail.com', 587)
    mailServer.ehlo()
    mailServer.starttls()
    mailServer.ehlo()
    mailServer.login(gmailUser, gmailPassword)
    mailServer.sendmail(gmailUser, recipient, msg.as_string())
    mailServer.close()



tableMySQL_Members = (
"CREATE TABLE Members ( "
"UserId INT UNSIGNED, "
"CONSTRAINT Members_PK PRIMARY KEY (UserId), "
"CONSTRAINT Members_UserId_FK FOREIGN KEY (UserId) REFERENCES main_db.Users (Id) "
"ON DELETE CASCADE "
"ON UPDATE CASCADE "
")"
)
tableMySQL_Chatrooms = (
"CREATE TABLE Chatrooms ( "
"Id INT UNSIGNED AUTO_INCREMENT, "
"CONSTRAINT Chatrooms_PK PRIMARY KEY (Id) "
")"
)
tableMySQL_ChatroomMessages = (
"CREATE TABLE ChatroomMessages ( "
"Id INT UNSIGNED AUTO_INCREMENT, "
"ChatroomId INT UNSIGNED NOT NULL, "
"Message VARCHAR(500) NOT NULL, "
"CreatedDateTime DATETIME NOT NULL, "
"CreatorId INT UNSIGNED NOT NULL, "
"CONSTRAINT ChatroomMessages_PK PRIMARY KEY (Id), "
"CONSTRAINT ChatroomMessages_ChatroomId_FK FOREIGN KEY (ChatroomId) REFERENCES Chatrooms (Id) "
"ON DELETE CASCADE "
"ON UPDATE CASCADE, "
"CONSTRAINT ChatroomMessages_CreatorId_FK FOREIGN KEY (CreatorId) REFERENCES Members (UserId) "
"ON UPDATE CASCADE "
")"
)
tableMySQL_R_ChatroomMembers = (
"CREATE TABLE R_ChatroomMembers ( "
"ChatroomId INT UNSIGNED NOT NULL, "
"MemberId INT UNSIGNED NOT NULL, "
"CONSTRAINT R_ChatroomMembers_PK PRIMARY KEY (ChatroomId, MemberId), "
"CONSTRAINT R_ChatroomMembers_ChatroomId_FK FOREIGN KEY (ChatroomId) REFERENCES Chatrooms (Id) "
"ON DELETE CASCADE "
"ON UPDATE CASCADE, "
"CONSTRAINT R_ChatroomMembers_MemberId_FK FOREIGN KEY (MemberId) REFERENCES Members (UserId) "
"ON DELETE CASCADE "
"ON UPDATE CASCADE "
")"
)
tableMySQL_TodoItems = (
"CREATE TABLE TodoItems ( "
"Id INT UNSIGNED AUTO_INCREMENT, "
"Title VARCHAR(100) NOT NULL, "
"Description TEXT, "
"DueDateTime DATETIME, "
"CreatedDateTime DATETIME NOT NULL, "
"LastEditDateTime DATETIME NOT NULL, "
"CONSTRAINT TodoItems_PK PRIMARY KEY (Id)"
")"
)
tableMySQL_ForumThreads = (
"CREATE TABLE ForumThreads ( "
"Id INT UNSIGNED AUTO_INCREMENT, "
"Title VARCHAR(100) NOT NULL, "
"CreatorId INT UNSIGNED NOT NULL, "
"CreatedDateTime DATETIME NOT NULL, "
"LastEditDateTime DATETIME NOT NULL, "
"CONSTRAINT ForumThreads_PK PRIMARY KEY (Id), "
"CONSTRAINT ForumThreads_CreatorId_FK FOREIGN KEY (CreatorId) REFERENCES Members (UserId) "
"ON UPDATE CASCADE "
")"
)
tableMySQL_ForumPosts = (
"CREATE TABLE ForumPosts ( "
"Id INT UNSIGNED AUTO_INCREMENT, "
"ThreadId INT UNSIGNED NOT NULL, "
"CreatorId INT UNSIGNED NOT NULL, "
"Content TEXT NOT NULL, "
"CreatedDateTime DATETIME NOT NULL, "
"LastEditDateTime DATETIME NOT NULL, "
"CONSTRAINT ForumPosts_PK PRIMARY KEY (Id), "
"CONSTRAINT ForumPosts_ThreadId_FK FOREIGN KEY (ThreadId) REFERENCES ForumThreads (Id) "
"ON DELETE CASCADE "
"ON UPDATE CASCADE, "
"CONSTRAINT ForumPosts_CreatorId_FK FOREIGN KEY (CreatorId) REFERENCES Members (UserId) "
"ON UPDATE CASCADE "
")"
)