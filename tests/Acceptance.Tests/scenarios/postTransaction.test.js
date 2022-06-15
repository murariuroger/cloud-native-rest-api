import request from "supertest";

const transaction = require("../testdata/transaction.json");
const constants = require("../testdata/constants.json");
const transactionApi = request(constants.endpoint);

beforeAll(() => {
  transaction.TransactionId = "121312e2f345443";
});

afterAll(async () => {
  await transactionApi.delete(`/transactions/${transaction.TransactionId}`);
});

describe("POST /transactions", () => {
  it("should return status code 201 if succcessfully", (done) => {
    transactionApi
      .post("/transactions")
      .send(transaction)
      .expect("Content-Type", "application/json")
      .expect(201, done);
  });

  it("should return status code 400 if body is empty", (done) => {
    transactionApi
      .post("/transactions")
      .send()
      .expect("Content-Type", "application/json")
      .expect(400, done);
  });
});
