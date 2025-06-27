var WebGLBrowserCheck = {
  isMobile: function () {
    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    return /android|iPhone|iPad|iPod|webOS|BlackBerry|IEMobile|Opera Mini|Mobile|Tablet/i.test(
      userAgent
    )
      ? 1
      : 0;
  },
};

mergeInto(LibraryManager.library, WebGLBrowserCheck);
