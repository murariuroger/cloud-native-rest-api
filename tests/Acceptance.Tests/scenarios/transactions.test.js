import getEndpointFromArgs from "../common/utils";
import request from "supertest";

const endpoint = getEndpointFromArgs();
const notExistingTransactionId = "123df3443vse";

describe("Transactions", () => {
  const transaction = {
    TransactionId: "34fre2354f4",
    UserEmail: "user@test.com",
    Date: "2021-10-22T14:47:01.727+00:00",
    Status: "NEW",
    Price: 250,
    OrigQty: 0.132,
    ExecutedQty: 0,
    CummulativeQuoteQty: 0,
    TimeInForce: "GTC",
    Type: "LIMIT",
    OrderSide: "BUY",
    FailedReason: null,
    UsdtAmount: 33,
  };

  describe("POST /transactions", () => {
    describe("Given the operation is successfull", () => {
      it("Should return 201", (done) => {
        request(endpoint)
          .post("/transactions")
          .send(transaction)
          .expect("Content-Type", "application/json")
          .expect(201, done);
      });
    });
  });

  describe("GET /transactions/:id", () => {
    describe("Given the transaction does not exists", () => {
      it("Should return a 204", (done) => {
        request(endpoint)
          .get("/transactions/nonExistingId")
          .expect("Content-Type", "application/json")
          .expect(204, done);
      });
    });

    describe("Given the transaction exists", () => {
      it("Should return a 200 and transaction is present in body", (done) => {
        request(endpoint)
          .get(`/transactions/${transaction.TransactionId}`)
          .expect("Content-Type", "application/json")
          .expect(200)
          .then((response) => {
            expect(response.body).toStrictEqual(transaction);
            done();
          })
          .catch((err) => done(err));
      });
    });
  });

  describe("PUT /transactions/:id", () => {
    describe("Given the transaction exists and operation is successfull", () => {
      const transactionUpdate = {
        UserEmail: "userupdated@test.com",
        Date: "2020-10-22T14:47:01.727+00:00",
        Status: "PENDING",
        Price: 250,
        OrigQty: 0.132,
        ExecutedQty: 0,
        CummulativeQuoteQty: 0,
        TimeInForce: "GTC",
        Type: "LIMIT",
        OrderSide: "SELL",
        FailedReason: null,
        UsdtAmount: 33,
      };

      it("Should return 200", (done) => {
        request(endpoint)
          .put(`/transactions/${transaction.TransactionId}`)
          .send(transactionUpdate)
          .expect("Content-Type", "application/json")
          .expect(200, done);
      });

      it("Should update the transaction", (done) => {
        request(endpoint)
          .get(`/transactions/${transaction.TransactionId}`)
          .expect("Content-Type", "application/json")
          .expect(200)
          .then((response) => {
            expect(response.body).toStrictEqual({
              ...transactionUpdate,
              TransactionId: transaction.TransactionId,
            });
            done();
          })
          .catch((err) => done(err));
      });
    });

    describe("Given the transaction doesn't exists and operation is successfull", () => {
      const transactionUpdate = {
        UserEmail: "someuser@test.com",
        Date: "2020-10-22T14:47:01.727+00:00",
        Status: "NEW",
        Price: 250,
        OrigQty: 0.132,
        ExecutedQty: 0,
        CummulativeQuoteQty: 0,
        TimeInForce: "GTC",
        Type: "LIMIT",
        OrderSide: "SELL",
        FailedReason: null,
        UsdtAmount: 666,
      };

      it("Confirms transaction doesn't exists", (done) => {
        request(endpoint)
          .get(`/transactions/${notExistingTransactionId}`)
          .expect("Content-Type", "application/json")
          .expect(204, done);
      });

      it("Should return 200", (done) => {
        request(endpoint)
          .put(`/transactions/${notExistingTransactionId}`)
          .send(transactionUpdate)
          .expect("Content-Type", "application/json")
          .expect(200, done);
      });

      it("Should create the transaction", (done) => {
        request(endpoint)
          .get(`/transactions/${notExistingTransactionId}`)
          .expect("Content-Type", "application/json")
          .expect(200)
          .then((response) => {
            expect(response.body).toStrictEqual({
              ...transactionUpdate,
              TransactionId: notExistingTransactionId,
            });
            done();
          })
          .catch((err) => done(err));
      });
    });
  });

  describe("DELETE /transactions/:id", () => {
    describe("Given the operation is successfull", () => {
      it("Should return 204", (done) => {
        request(endpoint)
          .delete(`/transactions/${transaction.TransactionId}`)
          .expect("Content-Type", "application/json")
          .expect(204, done);
      });

      it("Deletes the transaction", (done) => {
        request(endpoint)
          .get(`/transactions/${transaction.TransactionId}`)
          .expect("Content-Type", "application/json")
          .expect(204, done);
      });
    });
  });
});

afterAll(() => {
  return request(endpoint).delete(`/transactions/${notExistingTransactionId}`);
});
