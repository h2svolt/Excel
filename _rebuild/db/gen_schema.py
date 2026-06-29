import re, sys

edmx = open(r'F:\Excel-on-Work-Updated\Excel-main\Models\dBModel.edmx', encoding='utf-8').read()

ssdl = re.search(r'<edmx:StorageModels>(.*?)</edmx:StorageModels>', edmx, re.S).group(1)
schema_ns = re.search(r'<Schema[^>]*Namespace="([^"]+)"', ssdl)

# ---- Tables ----
types_with_len = {'nvarchar','varchar','char','nchar','varbinary','binary'}
out = []
out.append("IF DB_ID('db_StagingExcel') IS NULL CREATE DATABASE db_StagingExcel;")
out.append("GO")
out.append("USE db_StagingExcel;")
out.append("GO")

entity_blocks = re.findall(r'<EntityType Name="([^"]+)">(.*?)</EntityType>', ssdl, re.S)
# Map EntitySet name -> table store name (some sets differ); use store:Name from EntitySet
set_to_table = {}
for m in re.finditer(r'<EntitySet Name="([^"]+)"\s+EntityType="[^"]*\.([^"]+)"([^>]*)>', ssdl):
    setname, etype, rest = m.group(1), m.group(2), m.group(3)
    storename = re.search(r'store:Name="([^"]+)"', rest)
    set_to_table[etype] = storename.group(1) if storename else setname

def col_sql(p):
    name = p['Name']
    t = p['Type']
    parts = [f'[{name}]']
    base = t
    if t in types_with_len and 'MaxLength' in p:
        ml = p['MaxLength']
        base = f'{t}({ml})'
    elif t == 'decimal' or t == 'numeric':
        pr = p.get('Precision','18'); sc = p.get('Scale','0')
        base = f'{t}({pr},{sc})'
    parts.append(base)
    if p.get('StoreGeneratedPattern') == 'Identity':
        parts.append('IDENTITY(1,1)')
    parts.append('NOT NULL' if p.get('Nullable') == 'false' else 'NULL')
    return ' '.join(parts)

for ename, body in entity_blocks:
    table = set_to_table.get(ename, ename)
    keys = re.findall(r'<PropertyRef Name="([^"]+)"', re.search(r'<Key>(.*?)</Key>', body, re.S).group(1)) if re.search(r'<Key>', body) else []
    props = []
    for pm in re.finditer(r'<Property\s+([^>]+?)/>', body):
        attrs = dict(re.findall(r'(\w+)="([^"]*)"', pm.group(1)))
        props.append(attrs)
    cols = [ '    ' + col_sql(p) for p in props ]
    if keys:
        cols.append('    CONSTRAINT [PK_%s] PRIMARY KEY (%s)' % (table, ', '.join('[%s]'%k for k in keys)))
    out.append(f"IF OBJECT_ID('[dbo].[{table}]','U') IS NULL")
    out.append(f"CREATE TABLE [dbo].[{table}] (")
    out.append(',\n'.join(cols))
    out.append(");")
    out.append("GO")

open(r'F:\Excel-on-Work-Updated\_rebuild\schema_tables.sql','w',encoding='utf-8').write('\n'.join(out))
print("Tables generated:", len(entity_blocks))
print("Sample table names:", ', '.join(sorted(set_to_table.values()))[:300])
