db.createUser({
  user: process.env.MONGO_LOUISMANAGER_USERNAME,
  pwd: process.env.MONGO_LOUISMANAGER_PASSWORD,
  roles: [
    {
      role: "readWrite",
      db: process.env.MONGO_INITDB_DATABASE,
    },
  ],
});

db = new Mongo().getDB(process.env.MONGO_INITDB_DATABASE);

db.createCollection("User", { capped: false });
db.createCollection("Entry", { capped: false });
