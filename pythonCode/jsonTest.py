import json
json_string = '{"firstName": "Victor", "lastName": "Victor"}'

parsedJSON = json.loads(json_string)

print(parsedJSON['firstName'])
