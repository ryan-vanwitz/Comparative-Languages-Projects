#lang scheme
; ---------------------------------------------
; DO NOT REMOVE OR CHANGE ANYTHING UNTIL LINE 26
; ---------------------------------------------

; zipcodes.scm contains all the US zipcodes.
; This file must be in the same folder as hw2.scm file.
; You should not modify this file. Your code
; should work for other instances of this file.
(require "zipcodes.scm")

; Helper function
(define (mydisplay value)
	(display value)
	(newline)
)

; Helper function
(define (line func)
        (display "--------- ")
        (display func)
        (display " ------------")
        (newline)
)

; ================ Solve the following functions ===================
;
; Copyright (c) 2024 Ryan Van Witzenburg and ChatGPT. All rights reserved.
;
; Return a list with only the negatives items
(define (negatives lst)
    (if (null? lst)
      '()
      (if (negative? (car lst))
          (cons (car lst) (negatives (cdr lst)))
          (negatives (cdr lst))
       )
  )
)

(line "negatives")
(mydisplay (negatives '()))  ; -> ()
(mydisplay (negatives '(-1)))  ; -> (-1)
(mydisplay (negatives '(-1 1 2 3 4 -4 5)))  ; -> (-1 -4)
(mydisplay (negatives '(1 1 2 3 4 4 5)))  ; -> ()
(line "negatives")
; ---------------------------------------------

; Returns true if the two lists have identical structure
; in terms of how many elements and nested lists they have in the same order
(define (struct lst1 lst2)
	(cond ((and (null? lst1) (null? lst2)) #t)           
        ((or (null? lst1) (null? lst2)) #f)           
        ((not (list? (car lst1)))                     
         (and (not (list? (car lst2))
                   )               
              (struct (cdr lst1) (cdr lst2))))       
        ((not (list? (car lst2)))#f)               
        (else (and (struct (car lst1) (car lst2))     
                   (struct (cdr lst1) (cdr lst2))))
        )
  )

(line "struct")
(mydisplay (struct '(a b c (c a b)) '(1 2 3 (a b c))))  ; -> #t
(mydisplay (struct '(a b c d (c a b)) '(1 2 3 (a b c))))  ; -> #f
(mydisplay (struct '(a b c (c a b)) '(1 2 3 (a b c) 0)))  ; -> #f

(line "struct")
; ---------------------------------------------

; Returns a list of two numeric values. The first is the smallest 
; in the list and the second is the largest in the list. 
; lst -- contains numeric values, and length is >= 1.
(define (minAndMax lst) 
	(list (minelt lst) (maxelt lst))
)

(define (maxelt lst)
  (if (= (length lst) 1)
      (car lst)
      (max (car lst) (maxelt (cdr lst)))
  )
)

(define (minelt lst)
  (if (= (length lst) 1)
      (car lst)
      (min (car lst) (minelt (cdr lst)))
  )
)


(line "minAndMax")
(mydisplay (minAndMax '(1 2 -3 4 2)))  ; -> (-3 4)
(mydisplay (minAndMax '(1)))  ; -> (1 1)
(line "minAndMax")
; ---------------------------------------------

; Returns a list identical to the first list, while having all elements
; that are inside nested loops taken out. So we want to flatten all elements and have
; them all in a single list. For example '(a (a a) a))) should become (a a a a)
(define (flatten lst)
  (cond ((null? lst) '())                      
        ((not (pair? lst)) (list lst))         
        (else (append (flatten (car lst))
                      (flatten (cdr lst)))
         )
    )
 )

(line "flatten")
(mydisplay (flatten '(a b c)))  ; -> (a b c)
(mydisplay (flatten '(a (a a) a)))  ; -> (a a a a)
(mydisplay (flatten '((a b) (c (d) e) f)))  ; -> (a b c d e f)
(line "flatten")
; ---------------------------------------------

; The paramters are two lists. The result should contain the cross product
; between the two lists: 
; The inputs '(1 2) and '(a b c) should return a single list:
; ((1 a) (1 b) (1 c) (2 a) (2 b) (2 c))
; lst1 & lst2 -- two flat lists.
(define (crossproduct lst1 lst2)
 (if (null? lst1)
      '()
      (append (combineValues (car lst1) lst2)
              (crossproduct (cdr lst1) lst2))
      )
  )

(define (combineValues x lst)
  (if (null? lst)
      '()
      (cons (list x (car lst))
            (combineValues x (cdr lst))
            )
      )
  )
  
(line "crossproduct")
(mydisplay (crossproduct '(1 2) '(a b c)))
(line "crossproduct")
; ---------------------------------------------

; Returns the first latitude and longitude of a particular zip code.
; if there are multiple latitude and longitude pairs for the same zip code,
; the function should only return the first pair. e.g. (53.3628 -167.5107)
; zipcode -- 5 digit integer
; zips -- the zipcode DB- You MUST pass the 'zipcodes' function
; from the 'zipcodes.scm' file for this. You can just call 'zipcodes' directly
; as shown in the sample example
(define (getLatLon zipcode zips)
   (cond ((null? zips) '())
        ((= (caar zips) zipcode) (cddddr (car zips)))
        (else (getLatLon zipcode (cdr zips)))))

(line "getLatLon")
(mydisplay (getLatLon 45056 zipcodes))
(line "getLatLon")
; ---------------------------------------------

; Returns a list of all the place names common to two states.
; placeName -- is the text corresponding to the name of the place
; zips -- the zipcode DB
(define (getCommonPlaces state1 state2 zips)
  (if (null? zips)
      '()
      (let ((entry (car zips))
            (rest (cdr zips)))
        (let ((place (cadr entry)))
          (cond ((and (string=? (caddr entry) state1)
                      (member place (getPlacesInState state2 rest)))
                 (addIfNotPresent place (getCommonPlaces state1 state2 rest)))
                ((and (string=? (caddr entry) state2)
                      (member place (getPlacesInState state1 rest)))
                 (addIfNotPresent place (getCommonPlaces state1 state2 rest)))
                (else (getCommonPlaces state1 state2 rest))
            )
          )
        )
      )
  )

(define (getPlacesInState state zips)
    (if (null? zips)
        '()
        (if (string=? (caddr (car zips)) state)
            (cons (cadr (car zips)) (getPlacesInState state (cdr zips)))
            (getPlacesInState state (cdr zips))
         )
     )
  )

 (define (addIfNotPresent place places)
    (if (member place places)
        places
        (cons place places)
     )
  )

(line "getCommonPlaces")
(mydisplay (getCommonPlaces "OH" "MI" zipcodes))
(line "getCommonPlaces")
; ---------------------------------------------

; #### Only for Graduate Students ####
; Returns a list of all the place names common to a set of states.
; states -- is list of state names
; zips -- the zipcode DB
(define (getCommonPlaces2 states zips)
	'("Oxford" "Franklin")
)

(line "getCommonPlaces2")
(mydisplay (getCommonPlaces2 '("OH" "MI" "PA") zipcodes))
(line "getCommonPlaces2")
; ---------------------------------------------

; Returns the number of zipcode entries for a particular state.
; state -- state
; zips -- zipcode DB
(define (zipCount state zips)
	(if (null? zips)
      0
      (if (string=? (caddr (car zips)) state)
          (+ 1 (zipCount state (cdr zips)))
          (zipCount state (cdr zips)))))

(line "zipCount")
(mydisplay (zipCount "OH" zipcodes))
(line "zipCount")
; ---------------------------------------------

; #### Only for Graduate Students ####
; Returns the distance between two zip codes in "meters".
; Use lat/lon. Do some research to compute this.
; You can find some info here: https://www.movable-type.co.uk/scripts/latlong.html
; zip1 & zip2 -- the two zip codes in question.
; zips -- zipcode DB
(define (getDistanceBetweenZipCodes zip1 zip2 zips)
	0
)

(line "getDistanceBetweenZipCodes")
(mydisplay (getDistanceBetweenZipCodes 45056 48122 zipcodes))
(line "getDistanceBetweenZipCodes")
; ---------------------------------------------

; Some sample predicates
(define (POS? x) (> x 0))
(define (NEG? x) (< x 0))
(define (LARGE? x) (>= (abs x) 10))
(define (SMALL? x) (not (LARGE? x)))

; Returns a list of items that satisfy a set of predicates.
; For example (filterList '(1 2 3 4 100) '(EVEN?)) should return the even numbers (2 4 100)
; (filterList '(1 2 3 4 100) '(EVEN? SMALL?)) should return (2 4)
; lst -- flat list of items
; filters -- list of predicates to apply to the individual elements

(define (filterList lst filters)
  (if (null? filters)
      lst
      (filterList (manipulateList lst (car filters)) (cdr filters))
   )
 )

(define (manipulateList lst predicate)
  (cond ((null? lst) '())
        ((predicate (car lst))
         (cons (car lst) (manipulateList (cdr lst) predicate)))
        (else (manipulateList (cdr lst) predicate))
     )
 )

(line "filterList")
(mydisplay (filterList '(1 2 3 11 22 33 -1 -2 -3 -11 -22 -33) (list POS?)))
(mydisplay (filterList '(1 2 3 11 22 33 -1 -2 -3 -11 -22 -33) (list POS? even?)))
(mydisplay (filterList '(1 2 3 11 22 33 -1 -2 -3 -11 -22 -33) (list POS? even? LARGE?)))
(line "filterList")
; ---------------------------------------------

