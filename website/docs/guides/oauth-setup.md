---
sidebar_position: 2
---

# OAuth 2.0 Setup

This guide walks you through setting up Google OAuth 2.0 credentials to enable C4G to access your Google Sheets.

## Overview

C4G uses the Google Sheets API to read configuration data from your spreadsheets. To access this API, you need to:

1. Create a Google Cloud project
2. Enable the Google Sheets API
3. Create OAuth 2.0 credentials
4. Download the credentials file
5. Configure C4G to use the credentials

## Step-by-Step Setup

### Step 1: Create a Google Cloud Project

1. Go to [Google Cloud Console](https://console.cloud.google.com/)

2. Sign in with your Google account

3. Click on the project dropdown at the top of the page

4. Click **"New Project"**

5. Enter project details:
   - **Project name**: `C4G Game Configs` (or any name you prefer)
   - **Organization**: Leave as default (optional)
   - **Location**: Leave as default (optional)

6. Click **"Create"**

7. Wait for the project to be created (usually takes a few seconds)

### Step 2: Enable Google Sheets API

1. In the Google Cloud Console, ensure your new project is selected

2. Navigate to **APIs & Services → Library**
   - Or use the search bar and type "Google Sheets API"

3. Search for "Google Sheets API"

4. Click on **"Google Sheets API"**

5. Click the **"Enable"** button

6. Wait for the API to be enabled (a few seconds)

### Step 3: Create OAuth 2.0 Credentials

1. Navigate to **APIs & Services → Credentials**

2. Click **"+ CREATE CREDENTIALS"** at the top

3. Select **"OAuth client ID"**

4. If prompted to configure the OAuth consent screen:
   - Click **"Configure Consent Screen"**
   - Select **"External"** user type
   - Click **"Create"**

   Fill in the OAuth consent screen:
   - **App name**: `C4G Game Configs`
   - **User support email**: Your email
   - **Developer contact email**: Your email
   - Click **"Save and Continue"**
   - Skip "Scopes" (click **"Save and Continue"**)
   - Skip "Test users" (click **"Save and Continue"**)
   - Click **"Back to Dashboard"**

5. Return to **Credentials** and click **"+ CREATE CREDENTIALS"** again

6. Select **"OAuth client ID"**

7. Configure the OAuth client:
   - **Application type**: Select **"Desktop app"**
   - **Name**: `C4G Desktop Client` (or any name)

8. Click **"Create"**

9. A dialog will appear with your client ID and client secret

### Step 4: Download Credentials File

1. In the credentials dialog, click **"Download JSON"**

2. Save the file with a recognizable name:
   - Example: `c4g_credentials.json`
   - Example: `google_sheets_oauth.json`

3. Store this file securely - **Never commit it to version control!**

4. Recommended location:
   ```
   YourUnityProject/
   └── Credentials/
       └── c4g_credentials.json
   ```

5. Add to `.gitignore`:
   ```
   # Google OAuth credentials
   Credentials/
   *_credentials.json
   ```

### Step 5: Configure C4G with Credentials

1. Open Unity Editor

2. Open C4G Settings:
   - `Window → C4G → Settings`

3. In the Settings window, locate **"Client Secret"** field

4. Click the folder icon or paste the full path to your credentials JSON file:
   ```
   C:/YourProject/Credentials/c4g_credentials.json
   ```

5. C4G will validate the credentials file

## Credentials File Format

The downloaded JSON file should look like this:

```json
{
  "installed": {
    "client_id": "YOUR_CLIENT_ID.apps.googleusercontent.com",
    "project_id": "your-project-id",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://oauth2.googleapis.com/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_secret": "YOUR_CLIENT_SECRET",
    "redirect_uris": ["http://localhost"]
  }
}
```

**Important**: C4G specifically looks for the `"installed"` section for desktop applications.

## First-Time Authorization Flow

When you first generate configs with C4G:

1. C4G will open your default web browser

2. Google will ask you to sign in (if not already signed in)

3. Google will show a consent screen:
   - **"This app isn't verified"** warning may appear
   - Click **"Advanced"**
   - Click **"Go to C4G Game Configs (unsafe)"** (this is your own app, it's safe)

4. Review the permissions:
   - C4G requests: "View your Google Spreadsheets"
   - This is read-only access

5. Click **"Allow"**

6. You'll see a success message in the browser

7. Return to Unity - C4G will now have access to your sheets

8. The authorization token is stored locally and you won't need to re-authorize unless you revoke access

## Security Best Practices

### DO ✅

- **Store credentials outside the Unity Assets folder** for easier `.gitignore` management
- **Add credentials files to `.gitignore`** immediately
- **Use a dedicated Google account** for game development if working in a team
- **Revoke and regenerate credentials** if accidentally exposed
- **Keep credentials in a secure location** with restricted access
- **Document the credentials location** for your team (but not the file itself)

### DON'T ❌

- **Never commit credentials to version control** (Git, SVN, etc.)
- **Never share credentials files** via email or messaging
- **Never hardcode credentials** in scripts
- **Don't use personal Google accounts** for production/team projects
- **Don't grant more permissions** than "Google Sheets API" read access

## Troubleshooting

### "Invalid credentials" error

**Cause**: Credentials file is not in the correct format

**Solution**:
- Re-download the credentials from Google Cloud Console
- Ensure you selected "Desktop app" (not "Web application")
- Verify the JSON file has an `"installed"` section

### "Access denied" error

**Cause**: Google Sheets API is not enabled

**Solution**:
- Go to Google Cloud Console
- Navigate to **APIs & Services → Library**
- Search for "Google Sheets API"
- Click "Enable"

### "Browser doesn't open for authorization"

**Cause**: Default browser not set or firewall blocking

**Solution**:
- Manually copy the authorization URL from Unity Console
- Paste it into your browser
- Complete the authorization flow

### "This app isn't verified" warning

**Cause**: Your OAuth app hasn't gone through Google's verification process

**Solution**:
- This is normal for personal/development apps
- Click "Advanced" → "Go to [your app name] (unsafe)"
- For production apps, consider completing Google's app verification

### "Credentials file not found"

**Cause**: File path is incorrect or file was moved

**Solution**:
- Verify the file exists at the specified path
- Use absolute paths (not relative)
- Check for typos in the file path
- Ensure Unity has read permissions for the file

## Revoking Access

If you need to revoke C4G's access to your Google Sheets:

1. Go to [Google Account Permissions](https://myaccount.google.com/permissions)

2. Find your C4G app in the list

3. Click on it and select **"Remove Access"**

4. You'll need to re-authorize the next time you use C4G

## Team Setup

For team projects:

1. **Option A**: Each developer creates their own credentials
   - Pros: Better security, individual access control
   - Cons: Each person must set up OAuth

2. **Option B**: Share a team service account (Advanced)
   - Use a Google Service Account instead of OAuth
   - Share the service account key securely
   - Not covered in this guide

**Recommended**: Option A for small teams, Option B for larger organizations

## Next Steps

- [Google Sheets Setup](./google-sheets-setup) - Create your first config sheet
- [Editor Workflow](./editor-workflow) - Learn how to generate configs in Unity
- [Troubleshooting](../help) - Common issues and solutions
