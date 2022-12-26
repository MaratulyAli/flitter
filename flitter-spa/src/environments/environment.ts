const assetsPath = 'assets/images';
const apiBaseUrl = 'http://localhost:5126/api';

export const environment = {
  production: false,

  api: {
    posts: `${apiBaseUrl}/posts`,
    users: `${apiBaseUrl}/users`,
    follows: `${apiBaseUrl}/follows`,
    polls: `${apiBaseUrl}/polls`
  },

  assets: {
    images: {
      logoBlack: assetsPath + '/logo-black.png',
      logoWhite: assetsPath + '/logo-white.png',
      user: assetsPath + '/user.png'
    }
  }
};
