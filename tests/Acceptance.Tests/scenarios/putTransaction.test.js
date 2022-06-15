import request from "supertest";

const transaction = require("../testdata/transaction.json");
const transactionUpdate = require("../testdata/transactionUpdate.json");
const constants = require("../testdata/constants.json");
const transactionApi = request(constants.endpoint);
const nonExistingTransactionId = "123df3443vs37";

beforeAll(async () => {
  transaction.TransactionId = "121312e2f232253";
  await transactionApi.post("/transactions").send(transaction);
});

afterAll(async () => {
  await transactionApi.delete(`/transactions/${transaction.TransactionId}`);
  await transactionApi.delete(`/transactions/${nonExistingTransactionId}`);
});

describe("PUT /transactions/{TransactionId}", () => {
  it("should return status code 200 upon successful completion for an existing transaction", async () => {
    await transactionApi
      .put(`/transactions/${transaction.TransactionId}`)
      .send(transactionUpdate)
      .expect("Content-Type", "application/json")
      .expect(200);
  });

  it("should return status code 200 upon successful completion for a non existing transaction", async () => {
    await transactionApi
      .put(`/transactions/${nonExistingTransactionId}`)
      .send(transactionUpdate)
      .expect("Content-Type", "application/json")
      .expect(200);
  });

  it("should successfully update an existing transaction", async () => {
    await transactionApi
      .get(`/transactions/${transaction.TransactionId}`)
      .expect("Content-Type", "application/json")
      .expect(200)
      .then((response) => {
        expect(response.body).toStrictEqual({
          ...transactionUpdate,
          TransactionId: transaction.TransactionId,
        });
      });
  });

  it("should successfully create a new transaction if transaction does not exists", async () => {
    await transactionApi
      .get(`/transactions/${nonExistingTransactionId}`)
      .expect("Content-Type", "application/json")
      .expect(200)
      .then((response) => {
        expect(response.body).toStrictEqual({
          ...transactionUpdate,
          TransactionId: nonExistingTransactionId,
        });
      });
  });
});
