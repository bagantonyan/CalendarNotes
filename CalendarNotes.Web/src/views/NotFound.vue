<template>
  <div style="text-align: center; margin-top: 100px">
    <n-result status="404" title="404" description="Страница не найдена">
      <template #footer>
        <n-button @click="router.push('/')">Вернуться на главную</n-button>
      </template>
    </n-result>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { NResult, NButton } from 'naive-ui'

const router = useRouter()
</script>

