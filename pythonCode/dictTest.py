myDict = {}

myDict['victor'] = "great"
myDict['emily'] = "really awesome"

keyIndex = ""
print("First list")
for key in myDict:
	print(myDict[key])
	if myDict[key] == "great":
		keyIndex = myDict[key]

del myDict[key]			

print("")
print("Second list")
for key in myDict:
	print(myDict[key])
