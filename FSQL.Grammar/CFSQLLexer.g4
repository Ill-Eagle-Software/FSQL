lexer grammar CFSQLLexer;

//language keywords
PRINT					: P R I N T;


INT_DECL				: 'int';
DEC_DECL				: 'num';
STR_DECL				: 'str';
DATE_DECL				: 'date';
VAR_DECL				: 'var';

FIRSTIDENTCHAR			: ALPHA | UNDERSCORE;
IDENTCHAR				: FIRSTIDENTCHAR | DIGIT;

ALPHA					: (UCASE | LCASE);
UCASE					: [A-Z]; //'A'..'Z';
LCASE					: [a-z]; ///'a'..'z';

MATH_OPERATOR_1			: CARET;
MATH_OPERATOR_2			: STAR		| SLASH;
MATH_OPERATOR_3		  	: PLUS		| MINUS; 

BOOL_OPERATOR			: AMP		| AMP AMP
						| PIPE		| PIPE PIPE
						;

COMPARE_OPERATOR		: EQ		| NEQ
						| LT		| GT
						;

STRING_OPERATOR			: PLUS ;

DATE_OPERATOR			: PLUS | MINUS;

TWODIGITS				: DIGIT DIGIT;
FOURDIGITS				: TWODIGITS TWODIGITS;

EQ						: EQUAL EQUAL;
NEQ						: BANG EQUAL;


LT						: '<';
GT						: '>';
BANG					: '!';
EQUAL					: '=';
AMP						: '&';
PIPE					: '|';
PLUS					: '+';
MINUS					: '-';
STAR					: '*';
CARET					: '^';
PERIOD					: '.';
LPAREN					: '(';
RPAREN					: ')';
COMMA					: ',';

HASH					: '#';
SLASH					: '/';
COLON					: ':';
UNDERSCORE				: '_';

SINGLE_LINE_COMMENT		: '//' INPUT_CHAR*						-> channel(HIDDEN);
DELIMITED_COMMENT		: '/*' .*? '*/'							-> channel(HIDDEN);

STRING					: REGULAR_STRING;
REGULAR_STRING			: '"'  (~["\\\r\n\u0085\u2028\u2029] | COMMON_CHAR)* '"';

CRLF					: '\r'? '\n'							-> channel(HIDDEN);
WS						: (' ' | '\t')+							-> channel(HIDDEN);
UINT					: (DIGIT)+;
TRUE					: 'true';
FALSE					: 'false';
AM						: A M;
PM						: P M;
//DECIMAL					: UINT '.' UINT*;

/* Fragments */


fragment INPUT_CHAR:       ~[\r\n\u0085\u2028\u2029];

fragment COMMON_CHAR
	: SIMPLE_ESC_SEQUENCE
	| HEX_ESC_SEQUENCE	
	;

fragment SIMPLE_ESC_SEQUENCE
	: '\\\'' //#EscapedSingleQuote
	| '\\"'  //#EscapedDoubleQuote
	| '\\\\' //#EscapedBackslash
	| '\\0'  //#EscapedNull
	| '\\a'  //#EscapedA
	| '\\b'  //#EscapedB
	| '\\f'  //#EscapedF
	| '\\n'  //#EscapedNewLine
	| '\\r'  //#EscapedCarriageReturn
	| '\\t'  //#EscapedTab
	| '\\v'  //#EscapedVTab
	;
	
fragment HEX_ESC_SEQUENCE
	: '\\x' HEX_DIGIT
	| '\\x' HEX_DIGIT HEX_DIGIT
	| '\\x' HEX_DIGIT HEX_DIGIT HEX_DIGIT
	| '\\x' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
	;

fragment HEX_DIGIT : DIGIT | [A-F] | [a-f];
fragment DIGIT : [0-9];

fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];

