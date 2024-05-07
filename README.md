# Storytime
### Discover a world of stories at your fingertips

## What it does
- Generates unique, personalized stories based on user preferences and input
- Provides an intuitive, user-friendly interface for generating stories as per need

## How we built it
- Azure OpenAI GPT4-Turbo and DALL-E is used for generating stories and images
- ASP .NET Core & Semantic Kernel is used for development

### Technical

###### SQLite Migration

```powershell
Add-Migration Initial -OutputDir 'Data/Migrations' -Context ApplicationDbContext
```