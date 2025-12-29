import http from 'k6/http';
import { check } from 'k6';
import { SharedArray } from 'k6/data';

const ASPIRE_RESOURCE = __ENV.ASPIRE_RESOURCE || 'http://localhost:8080';

console.log(`HOST: ${ASPIRE_RESOURCE}`);

// Reused byte buffer (allocated ONCE)
const payload = new SharedArray('1000-bytes', function () {
  const arr = new Uint8Array(1000);
  for (let i = 0; i < arr.length; i++) {
    arr[i] = i % 256;
  }
  return Array.from(arr);
});

export default function () {
  const url = `${ASPIRE_RESOURCE}/cubby/cachekey`;

  const res = http.put(
    url,
    new Uint8Array(payload).buffer,
    {
      headers: {
        'Content-Type': 'application/octet-stream',
      },
    }
  );

  check(res, {
    'Status is Successful': (r) => r.status === 200 || r.status === 201,
  });
}
