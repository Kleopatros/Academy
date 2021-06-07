# Sends keystrokes to type the current date and time, dependent on system formats.
#
# Create a new Quick Action workflow in Automator with one Run AppleScript action.
# Then assign a Keyboard Shortcut to the action (found under the Services category).
on run {input, parameters}
    set now to (current date)
    set dateString to short date string of now
    set timeString to characters 1 thru 5 of (time string of now as string)
    
    tell application "System Events"
        keystroke dateString & " " & timeString
    end tell
end run