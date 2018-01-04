@ECHO OFF

C:

NET USE L: /DELETE
NET USE L: \\Mac\TerryLewis\Source

NET USE K: /DELETE
NET USE K: \\Mac\TerryLewis

L:
CD L:\Antlr\AntlrTutorial\FSQL.Grammar

:Again
CLS

ECHO Processing FSQL Grammar...
ECHO.
java -jar antlr4-csharp-4.6.4-complete.jar ./FSQL.g4 -Dlanguage=CSharp_v4_5 -visitor
ECHO.
ECHO FSQL Processing Complete.
ECHO.

ECHO CTRL-C to exit or
pause
GOTO Again

