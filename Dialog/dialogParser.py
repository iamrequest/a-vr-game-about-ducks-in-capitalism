import sys

# Usage: python3 [thisFile] [text file containing rough dialog]
# 
# File layout/format:
# [P/W/A, depending on the speaker]: [dialog]
# Eg: 
#   P: Hi, I'm the player saying something
#   W: Hi, I'm the work duck, saying something else
def main():
    if(len(sys.argv) != 2):
        print("Usage: {} [Text file to parse]".format(sys.argv[0]))
    else:
        with open(sys.argv[1], 'r') as dialog_file:
            for line in dialog_file:
                parseLine(line)

# Print out the line for the current speaker
# Convert shorthand speaker identitiers (eg: P, W, A) into the appropriate enum int
def printSpeaker(line):
    speaker = line.split(':')[0]
    if(speaker == 'P'):
        print('    - currentSpeaker: 0')
    elif(speaker =='W'):
        print('    - currentSpeaker: 1')
    elif(speaker =='A'):
        print('    - currentSpeaker: 1')
    else:
        print('    - currentSpeaker: ?')

# For each line of dialog in the text file...
#  1. Print out the speaker line
#  2. Write out the NPC animation (always 0, will clean up in editor)
#  3. Print out the dialog. This likely needs cleaning up to be accepted by unity
def parseLine(line):
    printSpeaker(line)
    print('      npcAnimation: 0')

    dialog = line.split(':')
    if(len(dialog) > 1):
        dialogText = ':'.join(dialog[1:])
        print('      text: {}'.format(dialogText))
    else: 
        print('      text: ERROR, MISSING')
    

if __name__ == "__main__":
    main()

