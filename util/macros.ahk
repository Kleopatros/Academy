; https://www.autohotkey.com/
; Use Windows Task Scheduler to run task on startup or login. Run the AHK executable with the path
; to this script as the only startup argument.

; CTRL+ALT+Z
; Sends an ISO 8601 timestamp (e.g. 2020-12-31T12:27Z).
^!z::
FormatTime, UtcNow, %A_NowUTC%, yyyy-MM-ddTHH:mmZ
Send, %UtcNow%
return

; CTRL+ALT+D
; Sends a local short date and time (e.g. 'Thu Dec 31 12:25').
^!d::
FormatTime, LocalNow, %A_Now%, ddd MMM dd HH:mm
Send, %LocalNow%
return

; CTRL+ALT+X
; Sends a random 6-character alphanumeric string.
^!x::
Result =
loop 6
{
    Random, Toggle, 0, 1
    if Toggle = 1
        Random, Character, 48, 57 ; [0-9]
    else
        Random, Character, 97, 122 ; [a-z]

    Character := chr(Character)
    Result = %Result%%Character%
}

Send, %Result%
Result =
return