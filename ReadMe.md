# FSQL
## The File System Structured Query Language ##

## Introduction

Some years ago, I started thinking, "what if we could treat the file system like a SQL database, where every folder was a potential table, every file was a potential row, and various attributes of those files, such as *FileName*, *Size*, and date *Created* were treated as columns?"  Such a language would trivialize some common operations.

We regularly query database tables to find differences; why is it so difficult to do the same thing in the file system?  Now it's not! With FSQL, the File system Structured Query Language, you can now use your SQL skills to query and manipulate files as sets.

FSQL is modeled heavily after T-SQL and C#, with a few quirks of its own thrown in for good measure.

### NOTE ###

FSQL is just a baby. At version 0.0.1, it's not even officially "born" yet! You are welcome to try it out, but be aware that it is likely to be EXTREMELY buggy, and you won't get a lot of help when it crashes. The language design is still fluid, and practically any part of it is subject to change at any time without warning. 

That being said, any and all constructive feedback is appreciated. If you hate FSQL, then simply delete it, live your life, and be happy with my sincere appreciation for trying it out. If you like it, but think it could be better, then let's talk and see if we can create something wonderful together! If you absolutely love it and think it's perfect as it is, then you must really be on some good drugs!

Work in progress will continue on the master branch for a while longer. When things get a bit more stable, then I'll start using development branches to protect the master, but for now, that's just overhead that I don't want to deal with. I will, however, try not to push anything to github that doesn't work.

## Documentation

* [Language Reference](Documentation/LangRef.md)
* [Examples](Documentation/Examples.md)
