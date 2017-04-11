function upsert(docs) {
    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();

    if (!docs) throw new Error("The array is undefined or null.");

    var docsLength = docs.length;
    if (docsLength === 0) {
        getContext().getResponse().setBody(0);
    }

    var count = 0;
    tryCreate(docs[count], callback);

    function tryCreate(doc, callback) {
        var isAccepted = collection.upsertDocument(collectionLink, doc, callback);
        if (!isAccepted) getContext().getResponse().setBody(count);
    }

    function callback(err, doc, options) {
        if (err) throw err;
        ++ count;

        if (count >= docsLength) {
            getContext().getResponse().setBody(count);
        } else {
            tryCreate(docs[count], callback);
        }
    }
}