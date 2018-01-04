parser grammar CFSQLParser;

options { tokenVocab = CFSQLLexer; }

compileUnit
	:	EOF
	;

simpleStatement
	: assignmentStatement
	| variableDeclaration
	| printStatement
	;

printStatement
	:	PRINT LPAREN args=argumentList? RPAREN;

argumentList
	:	current=expression (COMMA inner=argumentList)?;

variableDeclaration
	: type=knownTypeDecl name=identifier;

assignmentStatement
	:	type=typeDecl? name=identifier EQUAL expression;

typeDecl
	:	knownTypeDecl
	|	VAR_DECL
	;

knownTypeDecl
	:	INT_DECL
	|	DEC_DECL
	|	STR_DECL
	|	DATE_DECL
	;

expression
	:	compareOperation
	|	stringOperation
	|	numericOperation
	|	dateTimeOperation
	;

compareOperation
	:	leftStr=stringAtom op=COMPARE_OPERATOR rightStr=stringAtom
	|	leftNum=numberAtom op=COMPARE_OPERATOR rightNum=numberAtom
	|	leftDate=dateTimeAtom op=COMPARE_OPERATOR rightDate=numberAtom
	;

stringOperation
	:	left=stringAtom op=STRING_OPERATOR right=stringAtom
	;

numericOperation
	:	left=numberAtom op=MATH_OPERATOR_1 right=numberAtom #RaiseToPower
	|	left=numberAtom op=MATH_OPERATOR_2 right=numberAtom #Multiplication
	|	left=numberAtom op=MATH_OPERATOR_3 right=numberAtom #Addition
	;

dateTimeOperation
	:	left=dateTimeAtom op=DATE_OPERATOR right=dateTimeAtom
	;

dateTimeAtom
	:	dateTimeLiteral
	|	dateLiteral
	|	timeLiteral
	|	identifier
	;

numberAtom
	: intAtom
	| decimalAtom
	;

intAtom
	:	integerLiteral
	|	identifier
	;

decimalAtom
	:	decimalLiteral
	|	identifier
	;

stringAtom
	:	stringLiteral
	|	identifier
	;

identifier
	: FIRSTIDENTCHAR (IDENTCHAR)*
	;


literals
	:	stringLiteral
	|	numberLiteral
	|	booleanLiteral
	|	dateTimeLiteral
	|	dateLiteral
	|	timeLiteral
	;

dateTimeLiteral
	:	HASH dateFormat WS? timeFormat HASH
	;

dateLiteral
	:	HASH dateFormat HASH
	;

timeLiteral
	:	HASH timeFormat HASH
	;

dateFormat
	: TWODIGITS SLASH TWODIGITS SLASH FOURDIGITS 
	;

timeFormat
	: TWODIGITS COLON TWODIGITS (COLON TWODIGITS)? ampm?
	;

ampm 
	: AM | PM;

booleanLiteral
	: TRUE
	| FALSE
	;

numberLiteral
	:	integerLiteral
	|	decimalLiteral
	;

integerLiteral
	:	MINUS? UINT
	;
	

decimalLiteral
	:	 MINUS? UINT PERIOD UINT
	;

stringLiteral
	: STRING
	;