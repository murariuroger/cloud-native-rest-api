import request from "supertest";

const transaction = require("../testdata/transaction.json");
const constants = require("../testdata/constants.json");
const transactionApi = request(constants.endpoint);

beforeAll(async () => {
  transaction.TransactionId = "121312e2f3453";
  await transactionApi.post("/transactions").send(transaction);
});

afterAll(async () => {
  await transactionApi.delete(`/transactions/${transaction.TransactionId}`);
});

describe("GET /transactions/{TransactionId}", () => {
  it("should return status code 200 and transaction is present in body", (done) => {
    transactionApi
      .get(`/transactions/${transaction.TransactionId}`)
      .expect("Content-Type", "application/json")
      .expect(200)
      .then((response) => {
        expect(response.body).toStrictEqual(transaction);
        done();
      })
      .catch((err) => done(err));
  });

  it("should return status code 204 for a non existing transaction", (done) => {
    transactionApi
      .get("/transactions/nonExistingId")
      .expect("Content-Type", "application/json")
      .expect(204, done);
  });
});
