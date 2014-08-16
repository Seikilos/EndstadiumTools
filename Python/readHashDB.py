import hashlib
import sys
import base64
from functools import partial
import HashHelper



if len(sys.argv) != 3:
	print("Pass xml file as argument and base path")
	exit(1)
	
xmlFile = sys.argv[1]
basePath = sys.argv[2]

HashHelper.xmlAndBasePathVerifier(xmlFile, basePath)

print("Checking "+xmlFile)


results = HashHelper.verifyXml(xmlFile, basePath)

print("Mismatched files "+str(len(results)))
	