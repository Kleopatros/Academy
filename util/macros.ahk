; Use Windows Task Scheduler to run task on startup or login. Run the AHK executable with the path
; to this script as the only startup argument.

; CTRL+SHIFT+T
; Types the local date and time, followed by an ISO 8601 timestamp, followed by a newline and tab.
^+t::
FormatTime, LocalDate, %A_Now%, yyyy-MM-dd
FormatTime, LocalTime, %A_Now%, HH:mm
FormatTime, UtcNow, %A_NowUTC%, yyyy-MM-ddTHH:mmZ
Send, %LocalDate%T^b%LocalTime% %A_DDD%^b ^i^=%UtcNow%^=^i{ENTER}{TAB}
return

; CTRL+SHIFT+ENTER
; Types two newlines followed by two tabs.
^+ENTER::
Send, {ENTER}{ENTER}+{TAB}+{TAB}
return