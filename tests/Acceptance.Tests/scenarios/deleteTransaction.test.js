import request from "supertest";

const transaction = require("../testdata/transaction.json");
const constants = require("../testdata/constants.json");
const transactionApi = request(constants.endpoint);

beforeAll(async () => {
  transaction.TransactionId = "2232e2f3454233";
  await transactionApi.post("/transactions").send(transaction);
});

describe("DELETE /transaction/{TransactionId}", () => {
  it("should return status code 204 upon successful completion", (done) => {
    transactionApi
      .delete(`/transactions/${transaction.TransactionId}`)
      .send(transaction)
      .expect("Content-Type", "application/json")
      .expect(204, done);
  });
});
