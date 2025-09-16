var WebGLSaveLoad = {
  SaveToLocalStorage: function(key, value) {
    localStorage.setItem(UTF8ToString(key), UTF8ToString(value));
  },

  LoadFromLocalStorage: function(key) {
    var value = localStorage.getItem(UTF8ToString(key));
    if (value) {
      // The browser's string is null-terminated, so we need to copy it
      var buffer = _malloc(lengthBytesUTF8(value) + 1);
      stringToUTF8(value, buffer, lengthBytesUTF8(value) + 1);
      return buffer;
    }
    return 0; // Return 0 for null
  }
};

mergeInto(LibraryManager.library, WebGLSaveLoad);