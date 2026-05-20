import os,sys,io
root = "./"
decks_dir = os.path.join(root, "Decks")
out_path = os.path.join(root, "Game", "DeckData.cs")
if not os.path.isdir(decks_dir):
    print('Decks folder not found:', decks_dir)
    sys.exit(1)

entries = []
for fn in sorted(os.listdir(decks_dir)):
    if not fn.lower().endswith('.ydk'):
        continue
    path = os.path.join(decks_dir, fn)
    name = os.path.splitext(fn)[0]
    main_ids = []
    side_ids = []
    side = False
    with io.open(path, 'r', encoding='utf-8', errors='ignore') as f:
        for raw in f:
            line = raw.strip()
            if not line:
                continue
            if line.startswith('#'):
                continue
            if line.lower() == '!side':
                side = True
                continue
            try:
                idv = int(line)
            except:
                continue
            if side:
                side_ids.append(idv)
            else:
                main_ids.append(idv)
    entries.append((name, main_ids, side_ids))

# build C# file
with io.open(out_path, 'w', encoding='utf-8') as f:
    f.write('using System;\n')
    f.write('using System.Collections.Generic;\n')
    f.write('namespace WindBot.Game\n')
    f.write('{\n')
    f.write('    public static class DeckData\n')
    f.write('    {\n')
    f.write('        public static IReadOnlyDictionary<string, int[][]> AiDecks { get; } = new Dictionary<string, int[][]>\n')
    f.write('        {')
    for name, main_ids, side_ids in entries:
        f.write(f'\n            ["{name}"] = [\n                [')
        if main_ids:
            f.write(', '.join(str(x) for x in main_ids))
        f.write('],\n                [')
        if side_ids:
            f.write(', '.join(str(x) for x in side_ids))
        f.write(']\n            ],')
    f.write('\n        };\n')
    f.write('    }\n')
    f.write('}\n')
print('Wrote', out_path)
