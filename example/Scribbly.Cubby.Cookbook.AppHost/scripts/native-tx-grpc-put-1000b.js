import grpc from 'k6/net/grpc';
import { check } from 'k6';
import { SharedArray } from 'k6/data';

const ASPIRE_RESOURCE = __ENV.ASPIRE_RESOURCE || 'http://localhost:8080';

console.log(`HOST: ${ASPIRE_RESOURCE}`);

const client = new grpc.Client();
client.load(['./protos'], 'cubby.proto');

// Reused byte buffer (allocated ONCE)
const payload = new SharedArray('1000-bytes', function () {
  const arr = new Uint8Array(1000);
  for (let i = 0; i < arr.length; i++) {
    arr[i] = i % 256;
  }
  return Array.from(arr);
});

export default function () {
  client.connect('localhost:5000', {
    plaintext: true,
  });

  const response = client.invoke(
    'cubby.Cubby/Put',
    {
      key: 'cachekey',
      value: new Uint8Array(payload),
    }
  );

  check(response, {
    'status is OK': (r) => r && r.status === grpc.StatusOK,
  });

  client.close();
}