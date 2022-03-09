// openingHours grammar v0.7.1
// https://wiki.openstreetmap.org/wiki/Key:openingHours/specification

grammar OpeningHours;

timeDomain : ruleSequence (anyRuleSeparator ruleSequence)* ;
ruleSequence : selectorSequence (SPACE ruleModifier)? ;

// Rule separators

anyRuleSeparator : normalRuleSeparator | additionalRuleSeparator | fallbackRuleSeparator ;
normalRuleSeparator : ';' SPACE ;
additionalRuleSeparator : ',' SPACE ;
fallbackRuleSeparator : SPACE '||' SPACE ;

// Rule modifiers 

ruleModifier : 'open' (SPACE comment)?
			 | ('closed' | 'off') (SPACE comment)?
			 | 'unknown' (SPACE comment)?
			 | comment
			 ;

// Selectors

selectorSequence : twentyFourSeven
				 | wideRangeSelectors SPACE smallRangeSelectors
				 | wideRangeSelectors
				 | smallRangeSelectors
				 ;

wideRangeSelectors : yearSelector? monthdaySelector? weekSelector? separatorForReadability?
					| comment ':'
					; 

smallRangeSelectors : weekdaySelector? SPACE? timeSelector? ;
separatorForReadability : ':' ;

// Time selector
timeSelector : timespan (',' SPACE? timespan)* ;
timespan : time
		 | time '+'
		 | time '-' extendedTime '+'
		 | time '-' extendedTime 
		 | time '-' extendedTime '/' minute
		 | time '-' extendedTime '/' hourMinutes
		 ;
time: hourMinutes | variableTime ;
extendedTime : extendedHourMinutes | variableTime ;
variableTime : event
			  | '(' event plusOrMinus hourMinutes ')' 
			  ;
event : 'dawn' | 'sunrise' | 'sunset' | 'dusk' ;

// Weekday selector 
weekdaySelector : weekdaySequence
				 | holidaySequence
				 | holidaySequence ',' weekdaySequence
				 | weekdaySequence ',' holidaySequence
				 | holidaySequence SPACE weekdaySequence
				 ;
weekdaySequence : weekdayRange ( ',' weekdayRange )* ;
weekdayRange : wday
			  | wday '-' wday
			  | wday '[' nthEntry ( ',' nthEntry )* ']'
			  | wday '[' nthEntry ( ',' nthEntry )* ']' dayOffset
			  ;
			
holidaySequence : holiday ( ',' holiday )* ;
holiday : publicHoliday dayOffset?
		| schoolHoliday
		;
publicHoliday : 'PH' ;
schoolHoliday : 'SH' ;
nthEntry : NTH
		  | NTH '-' NTH
		  | '-' NTH
		  ;
NTH : [1-5] ;
dayOffset : SPACE plusOrMinus POSITIVE_NUMBER SPACE 'day' 's'? ;

// Week selector
weekSelector : 'week' week ( ',' week )* ;
week : weeknum
	 | weeknum '-' weeknum
	 | weeknum '-' weeknum '/' POSITIVE_NUMBER
	 ;

// Month selector
monthdaySelector : monthdayRange ( ',' monthdayRange )* ;
monthdayRange : (YEAR SPACE)? month
			   | (YEAR SPACE)? month '-' month
			   | dateFrom
			   | dateFrom (dateOffset)? '+'
			   | dateFrom (dateOffset)? '-' dateTo (dateOffset)?
			   ;
dateOffset : plusOrMinus wday | dayOffset | plusOrMinus wday dayOffset ;
dateFrom : (YEAR SPACE)? month SPACE daynum 
//		  | variableDate
		  ;
dateTo : dateFrom | daynum ;
variableDate : 'easter' ;

// Year selector 
yearSelector : yearRange ( ',' yearRange)* ;
yearRange : YEAR
		   | YEAR '-' YEAR
		   | YEAR '-' YEAR '/' POSITIVE_NUMBER
		   | YEAR '+'
		   ;

// Basic elements
plusOrMinus : '+' | '-' ;
twentyFourSeven : '24/7' ;
hour :  '00' | '01' | '02' | '03' | '04' | '05' | '06' | '07' | '08' | '09' | '10' | '11' | '12' | '13' | '14' | '15' | '16' | '17' | '18' | '19' | '20' | '21' | '22' | '23' | '24' ;
extendedHour : hour | '25' | '26' | '27' | '28' | '29' | '30' | '31' | '32' | '33' | '34' | '35' | '36' | '37' | '38' | '39' | '40' | '41' | '42' | '43' | '44' | '45' | '46' | '47' | '48' ;
minute : '00' | '01' | '02' | '03' | '04' | '05' | '06' | '07' | '08' | '09' | '10' | '11' | '12' | '13' | '14' | '15' | '16' | '17' | '18' | '19' | '20' | '21' | '22' | '23' | '24' | '25' | '26' | '27' | '28' | '29' | '30' | '31' | '32' | '33' | '34' | '35' | '36' | '37' | '38' | '39' | '40' | '41' | '42' | '43' | '44' | '45' | '46' | '47' | '48' | '49' | '50' | '51' | '52' | '53' | '54' | '55' | '56' | '57' | '58' | '59' ;
hourMinutes : hour ':' minute ;
extendedHourMinutes : extendedHour ':' minute ;
wday : 'Su' | 'Mo' | 'Tu' | 'We' | 'Th' | 'Fr' | 'Sa' ;
daynum : '01' | '02' | '03' | '04' | '05' | '06' | '07' | '08' | '09' | '10' | '11' | '12' | '13' | '14' | '15' | '16' | '17' | '18' | '19' | '20' | '21' | '22' | '23' | '24' | '25' | '26' | '27' | '28' | '29' | '30' | '31' ;
weeknum : '01' | '02' | '03' | '04' | '05' | '06' | '07' | '08' | '09' | '10' | '11' | '12' | '13' | '14' | '15' | '16' | '17' | '18' | '19' | '20' | '21' | '22' | '23' | '24' | '25' | '26' | '27' | '28' | '29' | '30' | '31' | '32' | '33' | '34' | '35' | '36' | '37' | '38' | '39' | '40' | '41' | '42' | '43' | '44' | '45' | '46' | '47' | '48' | '49' | '50' | '51' | '52' | '53' ;
month : 'Jan' | 'Feb' | 'Mar' | 'Apr' | 'May' | 'Jun' | 'Jul' | 'Aug' | 'Sep' | 'Oct' | 'Nov' | 'Dec' ;
YEAR : '19'[0-9][0-9] | '2'[0-9][0-9][0-9] ;
POSITIVE_NUMBER : [1-9][0-9]* ;
comment : '"' (COMMENT_CHARACTER)+ '"' ;
COMMENT_CHARACTER : [^"] ;
SPACE : ' ' ;

