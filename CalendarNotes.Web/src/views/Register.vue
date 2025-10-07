<template>
  <div style="max-width: 400px; margin: 60px auto">
    <n-card title="Регистрация">
      <n-form ref="formRef" :model="formValue" :rules="rules">
        <n-form-item label="Email" path="email">
          <n-input v-model:value="formValue.email" />
        </n-form-item>
        <n-form-item label="Имя" path="firstName">
          <n-input v-model:value="formValue.firstName" />
        </n-form-item>
        <n-form-item label="Фамилия" path="lastName">
          <n-input v-model:value="formValue.lastName" />
        </n-form-item>
        <n-form-item label="Пароль" path="password">
          <n-input v-model:value="formValue.password" type="password" show-password-on="click" />
        </n-form-item>
        <n-space vertical :size="16">
          <n-button type="primary" block :loading="authStore.loading" @click="handleRegister">
            Зарегистрироваться
          </n-button>
          <n-button block @click="router.push('/login')">Уже есть аккаунт? Войти</n-button>
        </n-space>
      </n-form>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { NCard, NForm, NFormItem, NInput, NButton, NSpace, useMessage } from 'naive-ui'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const message = useMessage()

const formValue = ref({ email: '', firstName: '', lastName: '', password: '' })
const rules = {
  email: { required: true, message: 'Введите email', trigger: 'blur' },
  firstName: { required: true, message: 'Введите имя', trigger: 'blur' },
  lastName: { required: true, message: 'Введите фамилию', trigger: 'blur' },
  password: { required: true, message: 'Введите пароль', trigger: 'blur' }
}

async function handleRegister() {
  try {
    await authStore.register(formValue.value)
    message.success('Регистрация успешна!')
    router.push('/notes')
  } catch (error: any) {
    message.error(error.response?.data?.message || 'Ошибка регистрации')
  }
}
</script>

