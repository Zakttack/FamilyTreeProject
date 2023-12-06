# Family Tree Project
# Background
Initially, the family reunion committee is implementing a family tree in a word document and continually saving each update as PDF. After attending the reunion and looking at the physical copy of the tree, I thought to myself that I can automate this process by the applying the fundamemental concepts of Full-Stack Software programming. As a family reunion committee member, I am automating the family tree by writting a ASP.NET Core Full-Stack application that represents each family instance as a tree node in terms of Graph Theory as a MongoDB Collection record and in the DAO, Reading/Writing from a MongoDB Collection partitioned by family name as tree structure represented in Graph Theory.

# Resources
"2023 Pfingsten Book Alternate.docx": this is a word document that I am updating to save as a PDF.
"2023PfingstenBookAlternate.pdf": this is the file that is being uploaded for testing.
"appsettings.json": this is main settings of the project.
"firstEntrySample.json": shows a sample MongoDB Collection record

# FamilyTreeLibrary
This directory shows the server-side implementation of the project consisting the DAO and Service stages.
# FamilyTreeLibrary/Data
Represents the DAO Layer

# FamilyTreeLibrary/Models
Stores the logical/physical objects as model classes.

# FamilyTreeLibrary/Service
Represents the Service Layer

# FamilyTreeLibraryTest
This is a test project for the DAO and Service Layer of the server-side

# FamilyTreeSratch
This serves as scratch paper to be able to see stuff show up on the console.

# FamilyTreeAPI
Represent the Controller layer that can be tested using software such as postman.