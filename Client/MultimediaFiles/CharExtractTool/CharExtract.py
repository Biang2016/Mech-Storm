import os

chars = []

f = open("Chars", 'r')

try:
    for line in f:
        for char in line:
            if char not in chars:
                chars.append(char)
finally:
    f.close()
    f = open("Chars_Res.txt", mode="w")
    for c in chars:
        f.write(c)

    f.close()
