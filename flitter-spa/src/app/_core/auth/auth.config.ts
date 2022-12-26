import { AuthConfig } from "angular-oauth2-oidc";

export const authConfig: AuthConfig = {
    issuer: 'http://localhost:8080/auth/realms/test',
    clientId: 'frontend',
    responseType: 'code',
    redirectUri: window.location.origin,
    scope: 'openid profile email offline_access',
    requireHttps: false,
    showDebugInformation: true
};
