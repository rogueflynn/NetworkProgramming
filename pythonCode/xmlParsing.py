import xml.etree.ElementTree as Et

xmlData = "<data><email>vgvgonzalez8@gmail.com</email><message>hello</message></data>"

tree = Et.ElementTree(Et.fromstring(xmlData))
root = tree.getroot()

print root[1].text

