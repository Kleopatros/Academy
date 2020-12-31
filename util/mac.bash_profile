#!/usr/bin/env bash

### TERMINAL PROMPT
# https://draculatheme.com/terminal

git_branch() {
    git branch 2>/dev/null | sed -e '/^[^*]/d' -e 's/* \(.*\)/ (\1)/'
}

#PS1="\[\e[0;35m\]\D{%FT%T}\[\e[m\] "        # date and time
#PS1="\[\e[0;35m\]\t\[\e[m\] "               # time
PS1="\[\e[0;35m\]\u@\h:\[\e[m\]"            # user and host
PS1+="\[\e[1;35m\]\W\[\e[m\]"               # directory
PS1+="\[\e[0;36m\]\$(git_branch)\[\e[m\] "  # branch
PS1+="\[\e[0;31m\]$\[\e[m\] "               # prompt
export PS1;

### OTHER CONFIGURATIONS

if type brew &>/dev/null; then
  HOMEBREW_PREFIX="$(brew --prefix)"
  if [[ -r "${HOMEBREW_PREFIX}/etc/profile.d/bash_completion.sh" ]]; then
    source "${HOMEBREW_PREFIX}/etc/profile.d/bash_completion.sh"
  else
    for COMPLETION in "${HOMEBREW_PREFIX}/etc/bash_completion.d/"*; do
      [[ -r "$COMPLETION" ]] && source "$COMPLETION"
    done
  fi
fi

eval "$(rbenv init -)"

export PATH="/usr/local/opt/postgresql@9.6/bin:$PATH"

export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"  # This loads nvm
[ -s "$NVM_DIR/bash_completion" ] && \. "$NVM_DIR/bash_completion"  # This loads nvm bash_completion

if command -v pyenv 1>/dev/null 2>&1; then
  eval "$(pyenv init -)"
fi
