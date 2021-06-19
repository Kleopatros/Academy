function onOpen(e) {
   DocumentApp.getUi()
      .createMenu('Macros')
      .addItem('Insert Timestamp', 'insertTimestamp')
      .addToUi();  
}

function insertTimestamp() {
  var cursor = DocumentApp.getActiveDocument().getCursor();
  if (cursor) {
    var element = cursor.insertText(Utilities.formatDate(new Date(), 'America/Los_Angeles', 'EE MMM d HH:mm'));
    if (!element) {
      DocumentApp.getUi().alert('Cannot insert text at this cursor location.');
    }
  } else {
    DocumentApp.getUi().alert('Cannot find a cursor in the document.');
  }
}
