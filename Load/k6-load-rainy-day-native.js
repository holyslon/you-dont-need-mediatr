import http from 'k6/http';
import {
    check,
    sleep
} from 'k6';

export const options = {
    stages: [{
        duration: '2m',
        target: 5000
    }]
};

export default function () {
    let data = {
        target: 7
    };
    const res = http.post('http://localhost:8080/native/calculate', JSON.stringify(data), {
        headers: {
            'Content-Type': 'application/json'
        },
    });
    check(res, {
        'status was 200': (r) => r.status == 200
    });
    sleep(1)
}

import {
    textSummary
} from 'https://jslib.k6.io/k6-summary/0.0.2/index.js';
export function handleSummary(data) {
    console.log('Preparing the end-of-test summary...');

    return {
        'stdout': textSummary(data, {
            indent: ' ',
            enableColors: true
        }), // Show the text summary to stdout...
        'k6-load-rainy-day-native.json': JSON.stringify(data), // and a JSON with all the details...
    };
}