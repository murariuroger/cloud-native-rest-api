import request from "supertest";

const transaction = require("../testdata/transaction.json");
const transactionPartialUpdate = require("../testdata/transactionPartialUpdate.json");
const constants = require("../testdata/constants.json");
const transactionApi = request(constants.endpoint);
const nonExistingTransactionId = "123dfz3443vs37";

beforeAll(async () => {
  transaction.TransactionId = "121x312e2f232253";
  await transactionApi.post("/transactions").send(transaction);
});

describe("PATCH /transactions/{TransactionId}", () => {
  it("should return status code 200 upon successful completion for an existing transaction", async () => {
    await transactionApi
      .patch(`/transactions/${transaction.TransactionId}`)
      .send(transactionPartialUpdate)
      .expect("Content-Type", "application/json")
      .expect(200);
  });

  it("should do a partial update", async () => {
    await transactionApi
      .get(`/transactions/${transaction.TransactionId}`)
      .expect("Content-Type", "application/json")
      .expect(200)
      .then((response) => {
        expect(response.body.Price).toEqual(transactionPartialUpdate.Price);
        expect(response.body.UsdtAmount).toEqual(
          transactionPartialUpdate.UsdtAmount
        );
        expect(response.body.UserEmail).toEqual(transaction.UserEmail);
        expect(response.body.Date).toEqual(transaction.Date);
        expect(response.body.Status).toEqual(transaction.Status);
        expect(response.body.OrigQty).toEqual(transaction.OrigQty);
        expect(response.body.OrderSide).toEqual(transaction.OrderSide);
        expect(response.body.CummulativeQuoteQty).toEqual(
          transaction.CummulativeQuoteQty
        );
        expect(response.body.FailedReason).toEqual(transaction.FailedReason);
        expect(response.body.TimeInForce).toEqual(transaction.TimeInForce);
        expect(response.body.Type).toEqual(transaction.Type);
      });
  });

  it("should return status code 400 for a non existing transaction", async () => {
    await transactionApi
      .patch(`/transactions/${nonExistingTransactionId}`)
      .send(transactionPartialUpdate)
      .expect("Content-Type", "application/json")
      .expect(400);
  });
});
