parser grammar FSQLParser;

options { tokenVocab = FSQLLexer; }

/*
 * Parser Rules
 */

 script					: body=codeblock;
						

//paramblock				: LPAREN (identifier | variableAssignment)* RPAREN;
parameters				: LPAREN block=paramblock? RPAREN #WrappedParameters;
arguments				: LPAREN block=argumentblock? RPAREN #WrappedArguments;
paramblock				: current=identifier (COMMA next=paramblock)* #FunctionParameters;
argumentblock			: current=expression (COMMA next=argumentblock)* #FunctionArguments;

codeblock				: LBRACE body=statement* RBRACE;

statement				: stmt=statementWithSemicolon SEMI #semicolonedStatement
						| FUNC functionName=IDENT parms=parameters body=codeblock #FunctionDef 
						| ifElseStatement #ifelse
						| whileDoStatement #whiledef
						| simulateStatement #simulate
						;

statementWithSemicolon	: functionCall
						| mapStatement
						| unmapStatement
						| sqlStatement
						| assignmentStatement
						| exportStatement						
						| returnStatement
						| internalStatement
						;

internalStatement		: TRACE (on=ON|off=OFF) #ToggleTracing
						;


ifElseStatement			: IF LPAREN cond=booleanExpression RPAREN trueblock=codeblock (ELSE falseblock=codeblock)? #IfStatement;
whileDoStatement		: WHILE LPAREN cond=booleanExpression RPAREN body=codeblock #WhileStatement;
simulateStatement		: (SIMULATE | SIM) body=codeblock;

exportStatement			: EXPORT paramblock;
returnStatement			: RETURN value=expression;

mapStatement			: MAP toClause;
unmapStatement			: UNMAP alias=IDENT;

sqlStatement			: selectFrom
					    | selectInto
						| insertInto
						| deleteFrom
						;

selectInto				: SELECT INTO target=filespec source=basicSelect;
insertInto				: INSERT INTO target=filespec source=basicSelect;
deleteFrom				: DELETE FROM target=filespec (WHERE where=whereExpression)?;

selectFrom				: basicSelect
						  (FORMAT formatting=formatClause)?
						  (OUTPUT output=outputClause)?
						  #FSQLSelect
						;

basicSelect   			: SELECT attributes=attributeList
						  FROM   sources=fromTargetList
						  (WHERE where=whereExpression)?
						;

attributeList			: current=scopedAttribute (COMMA optional=attributeList)? #GetAttributeList;

fromTargetList			: currentTarget=joinClause (COMMA innerTarget=joinClause)*;

joinClause				: join=joinClauses #ImplementJoinClause;

joinClauses				: left=filespec JOIN right=filespec ON expr=booleanExpression (AS newAlias=IDENT)? #ComplexJoinClause
						| left=filespec #SimpleJoinClause
						;
						
//whereClause				: WHERE whereExpression;

whereExpression			: expr=booleanExpression #WhereBoolExpression
						| value=scopedAttribute isNegated=NOT? IN LPAREN subQuery=selectFrom RPAREN #WhereInSubquery
						;

formatClause			: AS formatSpec=format;

format					: FMT_TEXT #FormatAsText
						| FMT_JSON #FormatAsJSON
						| FMT_CSV  #FormatAsCSV
						| FMT_SCALAR #FormatAsScalar // Returns the value of the first column of the first row
						;

outputClause			: TO outputSpec=outputClauses;

outputClauses			: SCREEN							#OutputToScreen
						| CLIPBOARD							#OutputToClipboard
						| FILE filename=stringExpression	#OutputToFile
						;

filespec				: STRING							#FileSpecString
						| IDENT								#FileSpecAlias
						| STRING IDENT						#FileSpecWithAlias						
						;

toClause				: toClauseBase (COMMA toClauseBase)*;

toClauseBase			: alias=IDENT TO folder=stringExpression (WITH_FILTER filter=stringExpression)?		 #MapFolderTo;							



scopedAttribute			: scopedAttributeNamed
						| scopedAttributeRef;

scopedAttributeNamed	: ref=scopedAttributeRef (AS colName=IDENT)? #FSQLScopedAttribute;
scopedAttributeRef		: (obj=IDENT '.')? prop=fileAttribute #FSQLScopedAttributeRef;

fileAttribute			: FULLNAME #FullNameAttribute
						| PATH #PathAttribute
						| FILENAME #FileNameAttribute
						| NAMEONLY #NameOnlyAttribute
						| EXTENSION #ExtensionAttribute						
						| SIZE #SizeAttribute
						| CREATED #CreatedAttribute
						| UPDATED #UpdatedAttribute
						| CHECKSUM #ChecksumAttribute
						| MULT #AllAttributes
						; 

assignmentStatement		: variableAssignment
						;

variableAssignment		: id=identifier EQUAL expr=expression #AssignVariable;						
/*
filespec				: LBRACK PATH_ROOT? delimitedFilespec RBRACK;
delimitedFilespec		: PATH_DELIM? FILENAME (delimitedFilespec)*;			
*/


expression				: functionCall #FunctionInvocation
						| selectFrom #selectAsExpression
						| deleteFrom #deleteFromAsExpression
						| simulateStatement #simulateAsExpression
						| booleanExpression #expIgnore1
						| variableAssignment #expIgnore2
						| stringExpression #expIgnore3
						| mathExpr #expIgnore4;

nonBooleanExpression	: variableAssignment
						| stringExpression
						| mathExpr;

stringExpression		: left=stringExpression PLUS right=stringExpression #StringConcat
						| stringAtomRule #StringAtom						
						;

stringAtomRule			: STRING #StringLiteral
						| identifier #StringIdent
						| scopedAttribute #StringScopedAttribute
						;


mathExpr				: left=mathExpr op=POWER right=mathExpr #RaiseToPower
						| left=mathExpr op=MULT right=mathExpr #Multiply
						| left=mathExpr op=SLASH right=mathExpr #Divide
						| left=mathExpr op=PLUS right=mathExpr #Add
						| left=mathExpr op=MINUS right=mathExpr #Subtract
						| op=(PLUS | MINUS) right=mathExpr #UnarySign
						| numAtom #NumericAtom
						| identifier #NumericIdentifier
						| scopedAttribute #NumericScopedAttribute
						;

numAtom					: UINT   #Int
						| NUMBER #Number
						| LPAREN expr=mathExpr RPAREN #NumParens
						| identifier #NumericIdent
						;


// BOOLEAN GRAMMAR
//try to match all first level && (e.g. && not included in some sub-expression)
booleanExpression : left=orexpression (op=AND right=orexpression)* #BooleanAnd;

//try to match all first level || (e.g. || not included in some sub-expression)
orexpression : left=notexpression (op=OR right=notexpression)* #BooleanOr;

//there may or may not be first level ! in front of an expression
notexpression : NOT boolExpr=boolAtom #InvertBoolean | boolExpr=boolAtom #DontInvertBoolean;

//finally, we found either name, or a sub-expression
boolAtom				: identifier #BooleanIdent 
						| boolConstant #BoolIgnore1 
						| LPAREN expr=booleanExpression RPAREN #BoolParens
						| left=nonBooleanExpression IS_EQUAL_TO right=nonBooleanExpression #NumberIsEqualTo
						| left=nonBooleanExpression IS_GREATER right=nonBooleanExpression #NumberIsGreater
						| left=nonBooleanExpression IS_LESS right=nonBooleanExpression #NumberIsLesser
						| left=nonBooleanExpression IS_GREATER_OR_EQUAL right=nonBooleanExpression #NumberIsGreaterOrEqual
						| left=nonBooleanExpression IS_LESS_OR_EQUAL right=nonBooleanExpression #NumberIsLessOrEqual
						| EXISTS LPAREN variable=identifier RPAREN #ExistsFunction
						;

// END BOOLEAN


functionCall			: (fnUsr=IDENT|fnInt=internalFunction) args=arguments;
						

identifier				: AT id=IDENT #IdentifierWithAt
						| AT UNDERSCORE #InternalIdentifier
						| attr=scopedAttributeRef #IdentifierWithScopedAttribute;

boolConstant			: BOOL_TRUE #TrueConstant | BOOL_FALSE #FalseConstant;


internalFunction		: WRITELINE
						| WRITE
						| READLINE
						| READ
						| COREDUMP
						| TRACE LPAREN num=mathExpr RPAREN						
						;
