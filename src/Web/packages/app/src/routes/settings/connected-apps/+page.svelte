<script lang="ts">
  import * as Card from "$lib/components/ui/card";
  import * as AlertDialog from "$lib/components/ui/alert-dialog";
  import { Button } from "$lib/components/ui/button";
  import { Badge } from "$lib/components/ui/badge";
  import { Separator } from "$lib/components/ui/separator";
  import {
    Smartphone,
    Trash2,
    Loader2,
    BadgeCheck,
    ExternalLink,
  } from "lucide-svelte";
  import { formatDate } from "$lib/utils/formatting";
  import { list, revoke } from "$lib/api/generated/connectedApps.generated.remote";

  const appsQuery = $derived(list());

  let revokingId: string | null = $state(null);

  async function handleRevoke(grantId: string) {
    revokingId = grantId;
    try {
      await revoke(grantId);
    } finally {
      revokingId = null;
    }
  }

  function relativeTime(date: Date | string | undefined | null): string {
    if (!date) return "Never";
    const d = typeof date === "string" ? new Date(date) : date;
    const now = Date.now();
    const diff = now - d.getTime();
    const minutes = Math.floor(diff / 60_000);
    if (minutes < 1) return "Just now";
    if (minutes < 60) return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    if (days < 30) return `${days}d ago`;
    return formatDate(d);
  }
</script>

<div class="space-y-6">
  <div>
    <h2 class="text-lg font-semibold">Connected Apps</h2>
    <p class="text-sm text-muted-foreground">
      Apps you have authorized to access your data on this tenant.
    </p>
  </div>

  <Separator />

  {#if appsQuery.loading}
    <div class="flex items-center justify-center py-12">
      <Loader2 class="h-6 w-6 animate-spin text-muted-foreground" />
    </div>
  {:else if appsQuery.error}
    <Card.Root>
      <Card.Content class="py-8 text-center text-sm text-muted-foreground">
        Failed to load connected apps.
      </Card.Content>
    </Card.Root>
  {:else if !appsQuery.current?.length}
    <Card.Root>
      <Card.Content class="py-12 text-center">
        <Smartphone class="mx-auto mb-3 h-8 w-8 text-muted-foreground/50" />
        <p class="text-sm font-medium">No connected apps yet</p>
        <p class="mt-1 text-xs text-muted-foreground">
          Apps you authorize will appear here.
        </p>
      </Card.Content>
    </Card.Root>
  {:else}
    <div class="space-y-3">
      {#each appsQuery.current as app (app.grantId)}
        <Card.Root>
          <Card.Header>
            <div class="flex items-start justify-between gap-4">
              <div class="space-y-1 flex-1 min-w-0">
                <Card.Title class="flex items-center gap-2 flex-wrap">
                  <span class="truncate">
                    {app.clientName ?? app.clientId ?? "Unknown App"}
                  </span>
                  {#if app.isVerified}
                    <Badge variant="secondary" class="text-xs gap-1">
                      <BadgeCheck class="h-3 w-3" />
                      Verified
                    </Badge>
                  {:else}
                    <Badge variant="outline" class="text-xs">
                      Self-registered
                    </Badge>
                  {/if}
                </Card.Title>
                <Card.Description>
                  {#if app.clientUri}
                    <a
                      href={app.clientUri}
                      target="_blank"
                      rel="noopener noreferrer"
                      class="inline-flex items-center gap-1 text-xs hover:underline"
                    >
                      {app.clientUri}
                      <ExternalLink class="h-3 w-3" />
                    </a>
                  {/if}
                </Card.Description>
              </div>

              <div class="shrink-0">
                <AlertDialog.Root>
                  <AlertDialog.Trigger>
                    {#snippet child({ props })}
                      <Button
                        {...props}
                        variant="outline"
                        size="sm"
                        class="text-destructive border-destructive/30 hover:bg-destructive/10"
                        disabled={revokingId === app.grantId}
                      >
                        {#if revokingId === app.grantId}
                          <Loader2 class="h-3.5 w-3.5 animate-spin" />
                        {:else}
                          <Trash2 class="h-3.5 w-3.5" />
                        {/if}
                      </Button>
                    {/snippet}
                  </AlertDialog.Trigger>
                  <AlertDialog.Content>
                    <AlertDialog.Header>
                      <AlertDialog.Title>Revoke access</AlertDialog.Title>
                      <AlertDialog.Description>
                        Revoke {app.clientName ?? "this app"}'s access to your
                        data? The app will need to be re-authorized to regain
                        access.
                      </AlertDialog.Description>
                    </AlertDialog.Header>
                    <AlertDialog.Footer>
                      <AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
                      <AlertDialog.Action
                        onclick={() => handleRevoke(app.grantId!)}
                      >
                        Revoke
                      </AlertDialog.Action>
                    </AlertDialog.Footer>
                  </AlertDialog.Content>
                </AlertDialog.Root>
              </div>
            </div>
          </Card.Header>

          <Card.Content class="border-t pt-3">
            <div class="flex flex-wrap gap-x-6 gap-y-2 text-xs text-muted-foreground">
              <div>
                <span class="font-medium">Scopes:</span>
                {#each app.scopes ?? [] as scope}
                  <Badge variant="outline" class="ml-1 text-[10px]">
                    {scope}
                  </Badge>
                {/each}
              </div>
              {#if app.label}
                <div>
                  <span class="font-medium">Label:</span> {app.label}
                </div>
              {/if}
              <div>
                <span class="font-medium">Authorized:</span>
                {formatDate(app.createdAt)}
              </div>
              <div>
                <span class="font-medium">Last used:</span>
                {relativeTime(app.lastUsedAt)}
              </div>
            </div>
          </Card.Content>
        </Card.Root>
      {/each}
    </div>
  {/if}
</div>
