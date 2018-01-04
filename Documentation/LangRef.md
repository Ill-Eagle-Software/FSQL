## Language Reference ##

### { } (Code Block) ###

The fundamental code structure in FSQL is the **Code Block**. A code block is defined as zero, one, or more FSQL statements enclosed in curly braces. Every FSQL script is a code block. Code blocks also define the body of functions, the alternative paths of **if** statements, the body of **while** loops, etc.

An **Empty Code Block** (```{}```) is the smallest legal FSQL script possible. 

### " " (Strings) ###

String constants are defined by enclosing the string in double quotes. Special characters may be escaped by using the backslash.

Escape Sequence  | Character
-----------------|-------------------
\\\\             | Backslash
\t               | Tab
\r               | Carriage Return
\n               | NewLine
