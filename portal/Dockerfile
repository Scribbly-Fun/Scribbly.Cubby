# --- Build Stage ---
FROM node:20-alpine AS builder

# Enable Corepack to use pnpm
RUN corepack enable && corepack prepare pnpm@latest --activate

WORKDIR /app

# Copy lockfile and package.json first to leverage cache
COPY pnpm-lock.yaml package.json ./

# Install dependencies
RUN pnpm install --frozen-lockfile

# Copy the rest of the application
COPY . .

# Build the SvelteKit app (requires @sveltejs/adapter-node)
RUN pnpm run build

# Remove dev dependencies to save space
RUN pnpm prune --prod

# --- Production Stage ---
FROM node:20-alpine
WORKDIR /app

# Copy built assets and production node_modules
COPY --from=builder /app/build ./build
COPY --from=builder /app/node_modules ./node_modules
COPY package.json .

ENV NODE_ENV=production
EXPOSE 3000

CMD ["node", "build"]