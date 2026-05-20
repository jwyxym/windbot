import os,sys,io,json
root = "./"
dialogs_dir = os.path.join(root, "Dialogs")
out_path = os.path.join(root, "Game", "DialogData.cs")
if not os.path.isdir(dialogs_dir):
    print('Dialogs folder not found:', dialogs_dir)
    sys.exit(1)

entries = []
for fn in sorted(os.listdir(dialogs_dir)):
    if not fn.lower().endswith('.json'):
        continue
    path = os.path.join(dialogs_dir, fn)
    name = os.path.splitext(fn)[0]
    try:
        with io.open(path, 'r', encoding='utf-8', errors='ignore') as f:
            data = json.load(f)
    except Exception as e:
        print('Failed to parse', path, e)
        continue
    entries.append((name, data))

def esc(s):
    return s.replace('\\', '\\\\').replace('"', '\\"').replace('\r', '\\r').replace('\n', '\\n')

with io.open(out_path, 'w', encoding='utf-8') as f:
    f.write('using System.Collections.Generic;\n')
    f.write('using WindBot.Game.AI;\n')
    f.write('namespace WindBot.Game\n')
    f.write('{\n')
    f.write('    public static class DialogData\n')
    f.write('    {\n')
    f.write('        public static IReadOnlyDictionary<string, DialogsData> Dialog { get; } = new Dictionary<string, DialogsData>\n')
    f.write('        {\n')
    for name, data in entries:
        f.write(f'            ["{name}"] = new DialogsData\n')
        f.write('            {\n')
        for key, val in data.items():
            if val is None:
                continue
            if isinstance(val, list):
                # write string array
                items = ', '.join('"%s"' % esc(str(x)) for x in val)
                f.write(f'                {key} = new string[] {{ {items} }},\n')
            else:
                # assume string
                f.write(f'                {key} = "{esc(str(val))}",\n')
        f.write('            },\n')
    f.write('        };\n')
    f.write('    }\n')
    f.write('}\n')
print('Wrote', out_path)
