import http from "k6/http";
import { check, sleep } from "k6";
import { Rate, Trend } from "k6/metrics";

// Custom metrics
const errorRate = new Rate("errors");
const paymentTrend = new Trend("payment_duration");
const authTrend = new Trend("auth_duration");

export const options = {
  stages: [
    { duration: "30s", target: 100 }, // Ramp up to 100 users
    { duration: "2m", target: 500 }, // Ramp up to 500 users
    { duration: "10m", target: 1000 }, // Sustained load at 1000 users
    { duration: "1m", target: 0 }, // Ramp down
  ],
  thresholds: {
    http_req_duration: ["p(95)<150"], // 95% of requests under 150ms
    errors: ["rate<0.01"], // Error rate under 1%
    payment_duration: ["p(95)<150"],
    auth_duration: ["p(95)<150"],
  },
};

const BASE_URL = "http://localhost:5000";

export default function () {
  // 30% authentication requests, 70% payment requests
  if (Math.random() < 0.3) {
    testAuthentication();
  } else {
    testPayment();
  }

  sleep(1);
}

function testAuthentication() {
  const payload = JSON.stringify({
    username: "test@example.com",
    password: "password123",
  });

  const params = {
    headers: {
      "Content-Type": "application/json",
      "X-Correlation-ID": generateCorrelationId(),
    },
  };

  const start = Date.now();
  const response = http.post(`${BASE_URL}/v1/auth/login`, payload, params);
  const duration = Date.now() - start;

  authTrend.add(duration);

  const success = check(response, {
    "auth status is 200": (r) => r.status === 200,
    "auth response has jwt": (r) => {
      try {
        const body = JSON.parse(r.body);
        return body.isSuccess && body.jwt;
      } catch {
        return false;
      }
    },
  });

  errorRate.add(!success);
}

function testPayment() {
  const payload = JSON.stringify({
    amount: Math.round((Math.random() * 1000 + 10) * 100) / 100,
    currency: "USD",
    destinationAccount:
      "ACC" + Math.random().toString(36).substr(2, 9).toUpperCase(),
  });

  const params = {
    headers: {
      "Content-Type": "application/json",
      Authorization: "Bearer mock-jwt-token-12345",
      "X-Correlation-ID": generateCorrelationId(),
    },
  };

  const start = Date.now();
  const response = http.post(`${BASE_URL}/v1/payments`, payload, params);
  const duration = Date.now() - start;

  paymentTrend.add(duration);

  const success = check(response, {
    "payment status is 200": (r) => r.status === 200,
    "payment response has paymentId": (r) => {
      try {
        const body = JSON.parse(r.body);
        return body.isSuccess && body.paymentId;
      } catch {
        return false;
      }
    },
  });

  errorRate.add(!success);
}

function generateCorrelationId() {
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0;
    const v = c == "x" ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

export function handleSummary(data) {
  return {
    "summary.json": JSON.stringify(data, null, 2),
    stdout: `
Performance Test Results:
========================
Total Requests: ${data.metrics.http_reqs.count}
Request Rate: ${data.metrics.http_reqs.rate.toFixed(2)} req/s
Average Duration: ${data.metrics.http_req_duration.avg.toFixed(2)}ms
95th Percentile: ${data.metrics.http_req_duration["p(95)"].toFixed(2)}ms
Error Rate: ${(data.metrics.errors.rate * 100).toFixed(2)}%

Auth Requests: ${data.metrics.auth_duration.count || 0}
Payment Requests: ${data.metrics.payment_duration.count || 0}

Thresholds:
- 95th percentile < 150ms: ${
      data.metrics.http_req_duration["p(95)"] < 150 ? "PASS" : "FAIL"
    }
- Error rate < 1%: ${data.metrics.errors.rate < 0.01 ? "PASS" : "FAIL"}
    `,
  };
}

