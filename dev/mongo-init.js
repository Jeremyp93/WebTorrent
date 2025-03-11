db.createUser({
  user: "webtorrent",
  pwd: "mongo",
  roles: [
    {
      role: "readWrite",
      db: "webtorrent",
    },
  ]
});

db = new Mongo().getDB("webtorrent");

db.createCollection("User", { capped: false });

db.User.insert([
  {
    _id: UUID('5296cff2-9970-42dd-8f3e-e2688fb48601'),
    userName: "jeremy.proot@outlook.com",
    normalizedUserName: "JEREMY.PROOT@OUTLOOK.COM",
    email: "jeremy.proot@outlook.com",
    normalizedEmail: "JEREMY.PROOT@OUTLOOK.COM",
    emailConfirmed: true,
    passwordHash: "AQAAAAIAAYagAAAAEGARJwZ7q/G57ihI4O76DbXu/mrirZ9y3bz/dfJ9jpSduanFlWRG1LhKb1MEPSx+WA==",
    securityStamp: "XABV5FE7V2I2WYMKWDQUNRRRMPX7FL6H",
    concurrencyStamp: "e199daaa-746d-425e-9373-3809ffcd20a3",
    phoneNumber: null,
    phoneNumberConfirmed: false,
    twoFactorEnabled: false,
    lockoutEnd: null,
    lockoutEnabled: true,
    accessFailedCount: 0,
    version: 1,
    createdOn: ISODate("2024-02-01T20:29:25.619+00:00"),
    claims: [],
    roles: [],
    logins: [],
    tokens: [],
    firstName: "Test",
    lastName: "Proot",
    oAuthProviders: []
  }
]);
