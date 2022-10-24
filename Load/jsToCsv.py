import json
import csv
import datetime
import glob
import os

files = glob.glob("./*.json")

meaningfull = ["data_received count","data_received rate","data_sent count","data_sent rate","http_req_duration avg","http_req_duration max","http_req_duration med","http_req_duration p(95)","http_reqs count","iteration_duration avg","iterations count","vus value","vus_max max"]

keys = []

values = {}

def pretty_duration(delta):
    if delta >= 1000:
        return ('%3.2f' % (delta/1000.0)) +" ms"
    else:
        return str(delta)+ " mls"

def parseValue(val, kind):
    if kind == "time":   
        return pretty_duration(int(val * 1000))
       # return str(datetime.timedelta(microseconds=int(val * 1000)))
    return str(val)


for filename in files:
#Opening JSON file
    data = {} 
    with open(filename) as f:
        data = json.load(f) 

    run_name = os.path.basename(filename)[:-5]
    full_name = run_name
    values[full_name]={}
    for name in data['metrics']:
        kind = data['metrics'][name]['contains']
        for key in data['metrics'][name]['values']:
            full_key = name + " " + key
            values[full_name][full_key] = parseValue(data['metrics'][name]['values'][key], kind)
            keys.append(full_key)

key_set = list(set(keys))
key_set.sort()

final_keys = [key for key in keys if key in meaningfull]

with  open('combined.csv', 'wb') as csvfile:
    writer = csv.writer(csvfile, delimiter=',', quotechar='|', quoting=csv.QUOTE_MINIMAL)
    header = ["name"]
    for key in final_keys:
        header.append(key)
    writer.writerow(header)
    for name in values:
        row = [name]
        for key in final_keys:                
            if key in values[name]:
                row.append(values[name][key])
            else:
                row.append("")
        writer.writerow(row)
