grammar Renpy;

tokens {
  INDENT,
  DEDENT,
  WITH,
  FADEIN,
  FADEOUT,
  JUMP,
  CALL,
  RETURN,
  MENU,
  DEFAULT,
  TRUE,
  FALSE
}

@lexer::header {
  using AntlrDenter;
}

@lexer::members {
  private DenterHelper denter;

  public override IToken NextToken()
  {
    if (denter == null)
    {
      denter = DenterHelper.Builder()
        .Nl(NL)
        .Indent(RenpyParser.INDENT)
        .Dedent(RenpyParser.DEDENT)
        .PullToken(base.NextToken);
    }

    return denter.NextToken();
  }
}

block:
  statement (NL* statement)*;

statement:
  label_def
  | define_def
  | scene_def
  | pause_def
  | play_music_def
  | stop_music_def
  | jump_def
  | call_def
  | menu_def
  | default_def
  | show_def
  | hide_def 
  | return_def
  | assignment
  | dialogue
  | narration
  | conditional_block
  | debug_def
  ;
debug_def:
  'debug' expression;

label_def:
  'label' IDENT (arguments)? ':' (INDENT block DEDENT)?
  ;

define_def:
  'define' IDENT '=' IDENT arguments
  ;

scene_def:
  'scene' IDENT ('with' (
    'dissolve'
    | 'fade'
    | 'crossfade'))? //TODO: Implement transitions
  ;

pause_def:
  'pause' NUMBER
  ;

play_music_def:
  'play music' IDENT ('fadein' NUMBER)?
  ;

stop_music_def:
  'stop music' IDENT ('fadeout' NUMBER)?
  ;

jump_def:
  'jump' IDENT (arguments)?
  ;

call_def:
  'call' IDENT (arguments)?
  ;

show_def:
  'show' IDENT IDENT
  ;

hide_def:
  'hide' STRING (arguments)?
  ;
  
menu_def:
  'menu' ':'
  INDENT
  (menu_option)*
  DEDENT
  ;

menu_option:
  STRING ':' INDENT block DEDENT
  ;

default_def:
  'default' IDENT '=' expression
  ;

return_def:
  'return'
  ;

dialogue:
  character_ref STRING
  ;

narration:
  STRING
  ;

character_ref:
  IDENT
  ;

arguments:
  '(' argument (',' argument)* ')'
  ;

argument:
  (IDENT '=')? expression   // Optional parameter hNUMBERing
  ;

conditional_block:
  'if' expression ':' INDENT block DEDENT
  (elif_block)*
  (else_block)?
  ;

elif_block:
  'elif' expression ':' INDENT block DEDENT
  ;

else_block:
  'else' ':' INDENT block DEDENT
  ;

assignment:
  '$' IDENT '=' expression
  ;

expression
    : additive
    | logical_or
    ;

// OR имеет самый низкий приоритет среди логических операций
logical_or
    : logical_and (OR logical_and)*
    ;

// AND — средний приоритет
logical_and
    : logical_not (AND logical_not)*
    ;

// NOT — унарный, высший приоритет среди логических
logical_not
    : NOT logical_not
    | relational
    ;

// Реляционные операции: поддерживают все сравнения
relational
    : additive (op+=(LT | GT | LE | GE | EQ | NE) additive)*
    ;


additive
    : multiplicative (op+=(PLUS | MINUS) multiplicative)*
    ;

multiplicative
    : primary (op+=(STAR | SLASH) primary)*
    ;

primary
    : literal
    | '(' expression ')'
    ;

literal:
  NUMBER
  | STRING
  | IDENT
  ;
IDENT: [a-zA-Z_][a-zA-Z0-9_]*;

STRING: '"' .*? '"'; // Simple string matching (improve for escaping)

PLUS  : '+';
MINUS : '-';
STAR  : '*';
SLASH : '/';
LT    : '<';
GT    : '>';
EQ    : '==';
NOT   : 'not';
AND   : 'and';
OR    : 'or';
NE    : '!=';
LE    : '<=';
GE    : '>=';
NUMBER: 
  [0-9]+ ('.' [0-9]+)? 
  | '.' [0-9]+ 
  | [0-9]+
  ;

NL:
  ('\r'? '\n' ' '*); // Note the ' '*

WS: [ \t]+ -> skip;

LINE_COMMENT: '#' ~[\r\n]* -> skip;
