import pymongo
from pymongo import MongoClient
from bson import Binary, Code
from bson.json_util import dumps 

client = MongoClient('localhost', 27017)

db = client['routing']

users = db['rTable']

for user in users.find():
	if user['name'] == 'Victor':
		print 'Found user'
	else:
		print 'Did not find user'
	print 'Name: ', user["name"], ' IP Address: ', user['ipaddr']



