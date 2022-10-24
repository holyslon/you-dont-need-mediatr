.PHONY: load raw
LOADS = k6-load-rainy-day-mediatr.csv k6-load-rainy-day-native.csv k6-load-sunny-day-mediatr.csv k6-load-sunny-day-native.csv
RAWS = k6-load-rainy-day-mediatr.json k6-load-rainy-day-native.json k6-load-sunny-day-mediatr.json k6-load-sunny-day-native.json

%.json: %.js
	docker run --rm -d -p 8080:80 --name test you-dont-need-mediatr:latest
	k6 run $<
	docker rm -fv test

%.csv: %js 

raw: $(RAWS)

load: $(LOADS)