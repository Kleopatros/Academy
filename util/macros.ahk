; https://www.autohotkey.com/
; Use Windows Task Scheduler to run task on startup or login. Run the AHK executable with the path
; to this script as the only startup argument.

; CTRL+SHIFT+T
; Sends a formatted local and global timestamp, then ENTER and TAB.
; Used to create logbooks with Microsoft OneNote.
^+t::
FormatTime, LocalDate, %A_Now%, yyyy-MM-dd
FormatTime, LocalTime, %A_Now%, HH:mm
FormatTime, UtcNow, %A_NowUTC%, yyyy-MM-ddTHH:mmZ
Send, %LocalDate%T^b%LocalTime% %A_DDD%^b ^i^=%UtcNow%^=^i{ENTER}{TAB}
return

; CTRL+SHIFT+ENTER
; Sends ENTER twice, then TAB twice.
; Used to create logbooks with Microsoft OneNote.
^+ENTER::
Send, {ENTER}{ENTER}+{TAB}+{TAB}
return

; CTRL+SHIFT+H
; Sends a random 8-character alpha-numeric string.
^+h::
loop 8
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
return