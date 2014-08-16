import sys
import HashHelper


if len(sys.argv) != 3:
	print("Pass xml file as argument and base path")
	exit(1)
	
xmlFile = sys.argv[1]
basePath = sys.argv[2]


print("Detecting new files "+basePath)

HashHelper.xmlAndBasePathVerifier(xmlFile, basePath)

allFiles = HashHelper.detectNewFiles(xmlFile, basePath)

# All remaining files are new
print("New files which are not in db: "+ str(len(allFiles)))

for a in allFiles:
	print(a)

exit(len(allFiles))





