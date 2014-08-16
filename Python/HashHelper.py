import hashlib
import sys
import base64
import os.path
from sets import Set
from functools import partial
import xml.etree.cElementTree as ET


# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
# Hashing functions
# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
def md5sum(filename):
    with open(filename, mode='rb') as f:
       d = hashlib.md5()
       for buf in iter(partial(f.read, 128), b''):
           d.update(buf)
    return d.digest()

def md5sumBase(filename):
	return base64.b64encode(md5sum(filename))
# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
# Utility for file existence
# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
def xmlAndBasePathVerifier(xmlFile, basePath):
	if not os.path.isfile(xmlFile) :
		print(xmlFile +" xml file does not exist")
		exit(1)

	if not os.path.isdir(basePath) :
		print(basePath +" basePath does not exist")
		exit(1)	
	
# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
# Verify files in xml
# returns mismatched files, contains file name, xml md5 and current md5
# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
def verifyXml(xmlFile, basePath):
	tree = ET.parse(xmlFile)
	root = tree.getroot()	

	results = []
	
	
	for child in root:
	  file = basePath+"\\"+child.find("name").text
	  #print(file)
	  #print(child.find("MD5").text)
	  hashFile = md5sumBase(file)
	  #print("Comparing "+hashFile +"="+child.find("MD5").text)
	  
	  xmlMD5 = child.find("MD5").text
	  
	  print("File \"{0}\" has xml hash {1} and current hash is {2}, matched = {3}".format(file, xmlMD5, hashFile, xmlMD5==hashFile))
	 
	  
	  if hashFile != xmlMD5:
		# not equal
		results.append([file,xmlMD5, hashFile])
	  
	return results


# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
# Detect new files not in xml
# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
def detectNewFiles(xmlFile, basePath):

	# Generate full file list from directory

	allFiles = Set()

	for root, dirs, files in os.walk(basePath) : 
		for file in files:
			allFiles.add(root+"\\"+file)
		
	# Remove xml file
	allFiles.remove(xmlFile)
	# Here all files are now a full paths to all given files

	#print("dumping allFiles")
	#for a in allFiles:
	#	if not os.path.isfile(a) :
	#		print("NOT EXIST")
	#	print(a)


	# Read xml and check for existence
	import xml.etree.ElementTree as ET
	tree = ET.parse(xmlFile)
	root = tree.getroot()	

	#print("dumping xml files")
	for child in root:
	  file = basePath+"\\"+child.find("name").text
	  #print(file)
	  if file in allFiles:
		# Remove it
		allFiles.remove(file)

	return allFiles

	
# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
# Detect new files not in xml
# ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
def detectNewFilesAndAdd(xmlFile, basePath):
	allFiles = detectNewFiles(xmlFile, basePath)
	
	tree = ET.parse(xmlFile)
	root = tree.getroot()	
	
	print len(root.findall("FILE_ENTRY"))
	
	for a in allFiles :
		newElement = ET.SubElement(root,"FILE_ENTRY")
		filename = ET.SubElement(newElement,"name")
		filename.text = a.replace(basePath+"\\","")
		md5 = ET.SubElement(newElement,"MD5")
		md5.text = md5sumBase(a)
		
	tree.write(xmlFile+".second.xml", "utf-8",True)
	
	
	
	
	return allFiles

