lexer grammar FSQLLexer;

/*
 * Lexer Rules
 */
SINGLE_LINE_COMMENT		: '//' InputCharacter*					-> channel(HIDDEN);
DELIMITED_COMMENT		: '/*' .*? '*/'							-> channel(HIDDEN);

STRING					: REGULAR_STRING;
REGULAR_STRING			: '"'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '"';

CRLF					: '\r'? '\n'							-> channel(HIDDEN);
WS						: (' ' | '\t')+			-> channel(HIDDEN);

//Internal Functions/Statements
WRITELINE				: W R I T E L I N E;
WRITE					: W R I T E;
READLINE				: R E A D L I N E;
READ					: R E A D;
COREDUMP				: C O R E D U M P;
TRACE					: T R A C E;
EXISTS					: E X I S T S;

//Keywords
SIMULATE				: S I M U L A T E;
SIM						: S I M;
MAP						: M A P;
UNMAP					: U N M A P;
TO						: T O;
WITH_FILTER				: W I T H WS F I L T E R;
CREATE					: C R E A T E;
INSERT					: I N S E R T;
DELETE					: D E L E T E;
SELECT					: S E L E C T;
INTO					: I N T O;
FROM					: F R O M;
JOIN					: J O I N;
ON						: O N;
OFF						: O F F;
WHERE					: W H E R E;
AS						: A S;
IN						: I N;

FORMAT				    : F O R M A T;
FMT_TEXT			    : T E X T;
FMT_JSON				: J S O N;
FMT_XML					: X M L;
FMT_CSV					: C S V;
FMT_SCALAR				: S C A L A R;
FMT_FIXED				: F I X E D;

OUTPUT					: O U T P U T;
FILE					: F I L E;
SCREEN					: S C R E E N;
CLIPBOARD				: C L I P B O A R D;

//attributes
FULLNAME				: F U L L N A M E;
PATH					: P A T H;
FILENAME				: F I L E N A M E;
NAMEONLY				: N A M E O N L Y;
EXTENSION				: E X T E N S I O N;
SIZE					: S I Z E;
CREATED					: C R E A T E D;
UPDATED					: U P D A T E D;
CHECKSUM				: C H E C K S U M;

FUNC					: F U N C;
EXPORT					: E X P O R T;
RETURN					: R E T U R N;

BOOL_TRUE				: T R U E;
BOOL_FALSE				: F A L S E;
NOT						: N O T;
AND						: A N D;
OR						: O R;

WHILE					: W H I L E;
DO						: D O ;
IF						: I F;
ELSE					: E L S E;
IS_EQUAL_TO				: EQUAL EQUAL;
IS_GREATER				: '>';
IS_LESS					: '<';
IS_GREATER_OR_EQUAL		: '<=';
IS_LESS_OR_EQUAL		: '>=';

/*
PATH_DELIM				: SLASH | BACKSLASH;
PATH_ROOT				: UNIX_ROOT | WINDOWS_ROOT;
UNIX_ROOT				: SLASH;
WINDOWS_ROOT			: BACKSLASH BACKSLASH;
FILENAME				: FilenameCharacter+;
*/

IDENT			: ((ALPHA)(ALPHA | DIGIT)*);

ALPHA			: (UCASE | LCASE);
UCASE			: [A-Z]; //'A'..'Z';
LCASE			: [a-z]; ///'a'..'z';

//----------------------------------------------------------------------------
// Symbols
//----------------------------------------------------------------------------
FULLSTOP 		: '.';
SEMI			: ';';
COMMA           : ','   ;
BACKSLASH		: '\\'  ;
LBRACK          : '['   ; 
RBRACK          : ']'   ;
PLUS            : '+'   ;
MINUS           : '-'   ;
LPAREN          : '('   ;
RPAREN          : ')'   ;
POWER			: '^'	;
MULT			: '*'   ;
SLASH			: '/'	;
EQUAL           : '='   ;
AMP				: '&'	;
AT              : '@'   ;
LBRACE          : '{'   ;
RBRACE          : '}'   ;
UNDERSCORE		: '_'	;
UINT			: (DIGIT)+;
NUMBER			: UINT '.' UINT*;

/* Fragments */
//fragment FilenameCharacter:       ~[*?\t\r\n\u0085\u2028\u2029];


fragment InputCharacter:       ~[\r\n\u0085\u2028\u2029];

fragment CommonCharacter
	: SimpleEscapeSequence
	| HexEscapeSequence	
	;

fragment SimpleEscapeSequence
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
	
fragment HexEscapeSequence
	: '\\x' HexDigit
	| '\\x' HexDigit HexDigit
	| '\\x' HexDigit HexDigit HexDigit
	| '\\x' HexDigit HexDigit HexDigit HexDigit
	;

fragment HexDigit : DIGIT | [A-F] | [a-f];

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
