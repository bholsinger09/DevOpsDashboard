const express = require('express');
const path = require('path');
const app = express();

// Serve static files from www directory
app.use(express.static(path.join(__dirname, 'www')));

// Security headers
app.use((req, res, next) => {
    res.setHeader('X-Content-Type-Options', 'nosniff');
    res.setHeader('X-Frame-Options', 'SAMEORIGIN');
    res.setHeader('X-XSS-Protection', '1; mode=block');
    next();
});

// SPA fallback - serve index.html for all routes
app.use((req, res) => {
  res.sendFile(path.join(__dirname, 'www', 'index.html'));
});

// Start server
const PORT = process.env.PORT || 8080;
app.listen(PORT, () => {
    console.log(`DevOps Dashboard running on port ${PORT}`);
});
